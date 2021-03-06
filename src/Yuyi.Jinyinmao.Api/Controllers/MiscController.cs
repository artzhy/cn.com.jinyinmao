// ***********************************************************************
// Project          : io.yuyi.jinyinmao.server
// Author           : Siqi Lu
// Created          : 2015-05-25  4:38 PM
//
// Last Modified By : Siqi Lu
// Last Modified On : 2015-06-09  9:59 PM
// ***********************************************************************
// <copyright file="MiscController.cs" company="Shanghai Yuyi Mdt InfoTech Ltd.">
//     Copyright ©  2012-2015 Shanghai Yuyi Mdt InfoTech Ltd. All rights reserved.
// </copyright>
// ***********************************************************************

using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Moe.AspNet.Filters;
using Yuyi.Jinyinmao.Api.Models;
using Yuyi.Jinyinmao.Service.Dtos;
using Yuyi.Jinyinmao.Service.Interface;

namespace Yuyi.Jinyinmao.Api.Controllers
{
    /// <summary>
    ///     Class MiscController.
    /// </summary>
    public class MiscController : ApiControllerBase
    {
        private readonly IUserService userService;
        private readonly IVeriCodeService veriCodeService;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MiscController" /> class.
        /// </summary>
        /// <param name="veriCodeService">The veri code service.</param>
        /// <param name="userService">The user service.</param>
        public MiscController(IVeriCodeService veriCodeService, IUserService userService)
        {
            this.veriCodeService = veriCodeService;
            this.userService = userService;
        }

        /// <summary>
        ///     发送验证码
        /// </summary>
        /// <remarks>
        ///     一天(自然天)同种类型的验证码只能发送10次，防止恶意使用验证码
        /// </remarks>
        /// <param name="request">
        ///     验证码发送请求
        /// </param>
        /// <response code="200"></response>
        /// <response code="400">
        ///     请求格式不正确
        ///     <br />
        ///     UAS1:该手机号未注册
        ///     <br />
        ///     UAS2:该手机号已注册
        /// </response>
        /// <response code="401"></response>
        /// <response code="500"></response>
        [Route("SendVeriCode"), ActionParameterRequired, ActionParameterValidate(Order = 1), ResponseType(typeof(SendVeriCodeResponse))]
        public async Task<IHttpActionResult> SendAsync(SendVeriCodeRequest request)
        {
            string cellphone = request.Cellphone;

            if (request.Type == VeriCodeType.ResetPaymentPassword)
            {
                if (this.CurrentUser == null)
                {
                    return this.Unauthorized();
                }

                cellphone = this.CurrentUser.Cellphone;
            }

            SignUpUserIdInfo info = await this.userService.GetSignUpUserIdInfoAsync(request.Cellphone);

            if (request.Type == VeriCodeType.ResetLoginPassword && !info.Registered)
            {
                return this.BadRequest("UAS1:该手机号未注册");
            }

            if (request.Type == VeriCodeType.SignUp && info.Registered)
            {
                return this.BadRequest("UAS2:该手机号已注册");
            }

            SendVeriCodeResult result = await this.veriCodeService.SendAsync(cellphone, request.Type, this.BuildArgs());

            return this.Ok(result.ToResponse());
        }

        /// <summary>
        ///     验证验证码是否正确
        /// </summary>
        /// <remarks>
        ///     验证码只能验证失败3次，并且只能使用一次，验证码有效期为30分钟
        /// </remarks>
        /// <param name="request">
        ///     验证码验证请求
        /// </param>
        /// <response code="200"></response>
        /// <response code="400">请求格式不正确</response>
        /// <response code="500"></response>
        [Route("VerifyVeriCode"), ActionParameterRequired, ActionParameterValidate(Order = 1), ResponseType(typeof(VerifyVeriCodeResponse))]
        public async Task<IHttpActionResult> VerifyCodeAsync(VerifyVeriCodeRequest request)
        {
            VerifyVeriCodeResult result = await this.veriCodeService.VerifyAsync(request.Cellphone, request.Code, request.Type);

            return this.Ok(result.ToResponse());
        }
    }
}