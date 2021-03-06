// ***********************************************************************
// Project          : io.yuyi.jinyinmao.server
// File             : UserAuthController.cs
// Created          : 2015-08-13  15:17
//
// Last Modified By : Siqi Lu
// Last Modified On : 2015-08-18  14:05
// ***********************************************************************
// <copyright file="UserAuthController.cs" company="Shanghai Yuyi Mdt InfoTech Ltd.">
//     Copyright ©  2012-2015 Shanghai Yuyi Mdt InfoTech Ltd. All rights reserved.
// </copyright>
// ***********************************************************************

using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Security;
using Moe.AspNet.Filters;
using Moe.AspNet.Utility;
using Moe.Lib;
using Yuyi.Jinyinmao.Api.Filters;
using Yuyi.Jinyinmao.Api.Models;
using Yuyi.Jinyinmao.Api.Models.User;
using Yuyi.Jinyinmao.Domain.Commands;
using Yuyi.Jinyinmao.Domain.Dtos;
using Yuyi.Jinyinmao.Service.Dtos;
using Yuyi.Jinyinmao.Service.Interface;

namespace Yuyi.Jinyinmao.Api.Controllers
{
    /// <summary>
    ///     UserAuthController.
    /// </summary>
    [RoutePrefix("User/Auth")]
    public class UserAuthController : ApiControllerBase
    {
        private readonly IUserInfoService userInfoService;
        private readonly IUserService userService;
        private readonly IVeriCodeService veriCodeService;

        /// <summary>
        ///     Initializes a new instance of the <see cref="UserAuthController" /> class.
        /// </summary>
        /// <param name="userInfoService">The user information service.</param>
        /// <param name="userService">The user service.</param>
        /// <param name="veriCodeService">The veri code service.</param>
        public UserAuthController(IUserInfoService userInfoService, IUserService userService, IVeriCodeService veriCodeService)
        {
            this.userInfoService = userInfoService;
            this.userService = userService;
            this.veriCodeService = veriCodeService;
        }

        /// <summary>
        ///     实名认证（通过银联）
        /// </summary>
        /// <remarks>
        ///     实名认证必须先设置支付密码
        ///     <br />
        ///     实名认证过程会绑定一张银行卡
        /// </remarks>
        /// <param name="request">
        ///     实名认证请求
        /// </param>
        /// <response code="200"></response>
        /// <response code="400">
        ///     请求格式不合法
        ///     <br />
        ///     UAA1:无法开通快捷支付功能
        ///     <br />
        ///     UAA2:请先设置支付密码
        ///     <br />
        ///     UAA3:已经通过实名认证
        ///     <br />
        ///     UAA4:该身份信息已经被绑定
        ///     <br />
        ///     UAA5:该银行卡已经被绑定
        /// </response>
        /// <response code="401">AUTH:请先登录</response>
        /// <response code="500"></response>
        [Route("Authenticate")]
        [CookieAuthorize]
        [ActionParameterRequired]
        [ActionParameterValidate(Order = 1)]
        public async Task<IHttpActionResult> Authenticate(AuthenticationRequest request)
        {
            request.CredentialNo = request.CredentialNo.ToUpperInvariant();

            if (await this.userInfoService.CheckCredentialNoUsedAsync(request.CredentialNo.ToUpperInvariant()))
            {
                return this.BadRequest("UAA4:该身份信息已经被绑定");
            }

            if (await this.userInfoService.CheckBankCardUsedAsync(request.BankCardNo))
            {
                return this.BadRequest("UAA5:该银行卡已经被绑定");
            }

            UserInfo userInfo = await this.userInfoService.GetUserInfoAsync(this.CurrentUser.Id);
            if (userInfo == null)
            {
                return this.BadRequest("USERNOTFOUND:用户信息加载错误，请重新登陆");
            }

            if (userInfo.Closed)
            {
                return this.BadRequest("UC:该账户已经被锁定，请联系金银猫客服");
            }

            if (!userInfo.HasSetPaymentPassword)
            {
                return this.BadRequest("UAA2:请先设置支付密码");
            }

            if (userInfo.Verified)
            {
                return this.BadRequest("UAA3:已经通过实名认证");
            }

            AddBankCard addBankCardCommand = this.BuildAddBankCardCommand(request);

            Authenticate authenticateCommand = this.BuildAuthenticateCommand(request);

            await this.userService.AuthenticateAsync(addBankCardCommand, authenticateCommand);

            return this.Ok();
        }

        /// <summary>
        ///     手机号是否已注册
        /// </summary>
        /// <remarks>
        ///     如果手机号已经注册过，则不能再用于注册
        /// </remarks>
        /// <param name="cellphone">
        ///     手机号
        /// </param>
        /// <response code="200"></response>
        /// <response code="400">UACC:手机号格式不正确</response>
        /// <response code="500"></response>
        [HttpGet]
        [Route("CheckCellphone")]
        [ActionParameterRequired]
        [ActionParameterValidate(Order = 1)]
        [ResponseType(typeof(CheckCellphoneResult))]
        public async Task<IHttpActionResult> CheckCellphone(string cellphone)
        {
            cellphone = cellphone ?? "";
            Match match = RegexUtility.CellphoneRegex.Match(cellphone);
            if (!match.Success || match.Index != 0 || match.Length != cellphone.Length)
            {
                return this.BadRequest("UACC:手机号格式不正确");
            }

            CheckCellphoneResult result = await this.userService.CheckCellphoneAsync(cellphone);
            return this.Ok(result);
        }

        /// <summary>
        ///     检验支付密码
        /// </summary>
        /// <remarks>
        ///     必须登录
        /// </remarks>
        /// <param name="request">
        ///     检验支付密码请求
        /// </param>
        /// <response code="200"></response>
        /// <response code="400">
        ///     请求格式错误
        ///     <br />
        ///     UACPP1:请重置支付密码后再试
        ///     <br />
        ///     UACPP1:支付密码错误&lt;br&gt;错误5次会锁定支付功能
        /// </response>
        /// <response code="401">AUTH:请先登录</response>
        /// <response code="500"></response>
        [Route("CheckPaymentPassword")]
        [CookieAuthorize]
        [ActionParameterRequired]
        [ActionParameterValidate(Order = 1)]
        public async Task<IHttpActionResult> CheckPaymentPassword(CheckPaymentPasswordRequest request)
        {
            CheckPaymentPasswordResult result = await this.userService.CheckPaymentPasswordAsync(this.CurrentUser.Id, request.Password);

            if (result.Lock)
            {
                return this.BadRequest("UACPP1:请重置支付密码后再试");
            }

            return !result.Success ? this.BadRequest("UACPP1:支付密码错误<br>错误5次会锁定支付功能")
                : this.Ok();
        }

        /// <summary>
        ///     重置登录密码
        /// </summary>
        /// <remarks>
        ///     重置密码前，必须要认证手机号，并且获得认证手机号的token
        /// </remarks>
        /// <param name="request">
        ///     重置登录密码请求
        /// </param>
        /// <response code="200"></response>
        /// <response code="400">
        ///     请求格式不合法
        ///     <br />
        ///     UARLP1:该验证码已经失效，请重新获取验证码
        ///     <br />
        ///     UARLP2:手机号码不存在，密码修改失败
        /// </response>
        /// <response code="401">AUTH:请先登录</response>
        /// <response code="500"></response>
        [Route("ResetLoginPassword")]
        [ActionParameterRequired]
        [ActionParameterValidate(Order = 1)]
        public async Task<IHttpActionResult> ResetLoginPassword(ResetPasswordRequest request)
        {
            UseVeriCodeResult veriCodeResult = await this.veriCodeService.UseAsync(request.Token, VeriCodeType.ResetLoginPassword);
            if (!veriCodeResult.Result)
            {
                return this.BadRequest("UARLP1:该验证码已经失效，请重新获取验证码");
            }

            SignUpUserIdInfo info = await this.userService.GetSignUpUserIdInfoAsync(veriCodeResult.Cellphone);
            if (!info.Registered)
            {
                return this.BadRequest("UARLP2:手机号码不存在，密码修改失败");
            }

            await this.userService.ResetLoginPasswordAsync(this.BuildResetLoginPasswordCommand(request, info));

            return this.Ok();
        }

        /// <summary>
        ///     重置支付密码
        /// </summary>
        /// <remarks>
        ///     如果已经通过实名认证，需要验证手机号、用户姓名、身份证号3要素进行重置支付密码
        ///     <br />
        ///     如果没有实名认证过，只需要验证手机号即可进行实名认证
        /// </remarks>
        /// <param name="request">
        ///     重置支付密码请求
        /// </param>
        /// <response code="200"></response>
        /// <response code="400">
        ///     请求格式不合法
        ///     <br />
        ///     UARPP1:该验证码已经失效，请重新获取验证码
        ///     <br />
        ///     UARPP2:您输入的身份信息错误！请重新输入
        ///     <br />
        ///     UARPP3:支付密码不能与登录密码一致，请选择新的支付密码
        ///     <br />
        ///     UARPP4:无法重置支付密码
        /// </response>
        /// <response code="401">AUTH:请先登录</response>
        /// <response code="500"></response>
        [Route("ResetPaymentPassword")]
        [CookieAuthorize]
        [ActionParameterRequired]
        [ActionParameterValidate(Order = 1)]
        public async Task<IHttpActionResult> ResetPaymentPassword(ResetPaymentPasswordRequest request)
        {
            UserInfo userInfo = await this.userService.GetUserInfoAsync(this.CurrentUser.Id);
            if (userInfo == null)
            {
                return this.BadRequest("USERNOTFOUND:用户信息加载错误，请重新登陆");
            }

            if (userInfo.Closed)
            {
                return this.BadRequest("UC:该账户已经被锁定，请联系金银猫客服");
            }

            if (!userInfo.HasSetPaymentPassword ||
                (userInfo.Verified && (userInfo.RealName != request.UserRealName || !string.Equals(userInfo.CredentialNo, request.CredentialNo, StringComparison.InvariantCultureIgnoreCase))))
            {
                return this.BadRequest("UARPP2:您输入的身份信息错误！请重新输入");
            }

            if (await this.userService.CheckPasswordAsync(this.CurrentUser.Id, request.Password))
            {
                return this.BadRequest("UARPP3:支付密码不能与登录密码一致，请选择新的支付密码");
            }

            UseVeriCodeResult result = await this.veriCodeService.UseAsync(request.Token, VeriCodeType.ResetPaymentPassword);
            if (!result.Result)
            {
                return this.BadRequest("UARPP1:该验证码已经失效，请重新获取验证码");
            }

            await this.userService.SetPaymentPasswordAsync(this.BuildSetPaymentPasswordCommand(request));

            return this.Ok();
        }

        /// <summary>
        ///     设置支付密码
        /// </summary>
        /// <remarks>
        ///     支付密码必须包含一位字母或者一般特殊字符，长度为8到18位之间,并且不能与登录密码一致
        /// </remarks>
        /// <param name="request">
        ///     设置支付密码请求
        /// </param>
        /// <response code="200"></response>
        /// <response code="400">
        ///     请求格式不合法
        ///     <br />
        ///     UASPP1:支付密码不能与登录密码一致，请选择新的支付密码
        ///     <br />
        ///     UASPP2: 支付密码已经设置，请直接使用
        ///     <br />
        ///     UASPP3:无法设置支付密码
        /// </response>
        /// <response code="401">AUTH:请先登录</response>
        /// <response code="500"></response>
        [Route("SetPaymentPassword")]
        [CookieAuthorize]
        [ActionParameterRequired]
        [ActionParameterValidate(Order = 1)]
        public async Task<IHttpActionResult> SetPaymentPassword(SetPaymentPasswordRequest request)
        {
            if (await this.userService.CheckPasswordAsync(this.CurrentUser.Id, request.Password))
            {
                return this.BadRequest("UASPP1:支付密码不能与登录密码一致，请选择新的支付密码");
            }

            UserInfo userInfo = await this.userInfoService.GetUserInfoAsync(this.CurrentUser.Id);
            if (userInfo == null)
            {
                return this.BadRequest("USERNOTFOUND:用户信息加载错误，请重新登陆");
            }

            if (userInfo.Closed)
            {
                return this.BadRequest("UC:该账户已经被锁定，请联系金银猫客服");
            }

            if (userInfo.HasSetPaymentPassword)
            {
                return this.BadRequest("UASPP2: 支付密码已经设置，请直接使用");
            }

            await this.userService.SetPaymentPasswordAsync(this.BuildSetPaymentPasswordCommand(request));

            return this.Ok();
        }

        /// <summary>
        ///     登录
        /// </summary>
        /// <remarks>
        ///     通过账户名和密码登录，现在账户名即为用户的手机号
        /// </remarks>
        /// <param name="request">
        ///     登录请求
        /// </param>
        /// <response code="200"></response>
        /// <response code="400">请求格式不合法</response>
        /// <response code="500"></response>
        [Route("SignIn")]
        [ActionParameterRequired]
        [ActionParameterValidate(Order = 1)]
        [ResponseType(typeof(SignInResponse))]
        public async Task<IHttpActionResult> SignIn(SignInRequest request)
        {
            SignInResult signInResult = await this.userService.CheckPasswordViaCellphoneAsync(request.LoginName, request.Password);

            if (signInResult.Success)
            {
                this.SetCookie(signInResult.UserId, signInResult.Cellphone);
            }

            return this.Ok(signInResult.ToResponse());
        }

        /// <summary>
        ///     金银猫客户端注销接口
        /// </summary>
        /// <remarks>
        ///     客户端可以通过直接清除Cookie MA的值实现注销
        /// </remarks>
        /// <response code="200">注销成功</response>
        [HttpGet]
        [Route("SignOut")]
        public IHttpActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            return this.Ok();
        }

        /// <summary>
        ///     金银猫客户端注册接口
        /// </summary>
        /// <remarks>
        ///     在金银猫的客户端注册，包括PC网页、M版网页、iPhone、Android
        ///     <br />
        ///     前置条件：已经通过验证码验证手机号码的真实性
        /// </remarks>
        /// <param name="request">
        ///     注册请求
        /// </param>
        /// <response code="200"></response>
        /// <response code="400">
        ///     请求格式不合法
        ///     <br />
        ///     UASU1:请输入正确的验证码
        ///     <br />
        ///     UASU2:此号码已注册，请直接登录
        ///     <br />
        ///     UASU3:注册失败
        /// </response>
        /// <response code="500"></response>
        [Route("SignUp")]
        [ActionParameterRequired]
        [ActionParameterValidate(Order = 1)]
        [ResponseType(typeof(SignUpResponse))]
        public async Task<IHttpActionResult> SignUp(SignUpRequest request)
        {
            UseVeriCodeResult result = await this.veriCodeService.UseAsync(request.Token, VeriCodeType.SignUp);

            if (!result.Result)
            {
                return this.BadRequest("UASU1:请输入正确的验证码");
            }

            SignUpUserIdInfo info = await this.userService.GetSignUpUserIdInfoAsync(result.Cellphone);

            if (info.Registered)
            {
                return this.BadRequest("UASU2:此号码已注册，请直接登录");
            }

            UserInfo userInfo = await this.userService.RegisterUserAsync(this.BuildUserRegisterCommand(request, info));

            if (userInfo == null)
            {
                return this.BadRequest("UASU3:注册失败");
            }

            // 自动登陆
            this.SetCookie(userInfo.UserId, userInfo.Cellphone);

            return this.Ok(userInfo.ToSignUpResponse());
        }

        private AddBankCard BuildAddBankCardCommand(AuthenticationRequest request)
        {
            return new AddBankCard
            {
                EntityId = this.CurrentUser.Id,
                Args = this.BuildArgs(),
                BankCardNo = request.BankCardNo,
                BankName = request.BankName,
                CityName = request.CityName,
                UserId = this.CurrentUser.Id
            };
        }

        private Authenticate BuildAuthenticateCommand(AuthenticationRequest request)
        {
            return new Authenticate
            {
                EntityId = this.CurrentUser.Id,
                Args = this.BuildArgs(),
                BankCardNo = request.BankCardNo,
                BankName = request.BankName,
                Cellphone = this.CurrentUser.Cellphone,
                CityName = request.CityName,
                Credential = request.Credential,
                CredentialNo = request.CredentialNo.ToUpperInvariant(),
                RealName = request.RealName,
                UserId = this.CurrentUser.Id
            };
        }

        private ResetLoginPassword BuildResetLoginPasswordCommand(ResetPasswordRequest request, SignUpUserIdInfo info)
        {
            return new ResetLoginPassword
            {
                EntityId = info.UserId,
                Args = this.BuildArgs(),
                Password = request.Password,
                Salt = info.UserId.ToGuidString(),
                UserId = info.UserId
            };
        }

        private SetPaymentPassword BuildSetPaymentPasswordCommand(ResetPaymentPasswordRequest request)
        {
            return new SetPaymentPassword
            {
                EntityId = this.CurrentUser.Id,
                Args = this.BuildArgs(),
                Override = true,
                PaymentPassword = request.Password,
                Salt = this.CurrentUser.Id.ToGuidString(),
                UserId = this.CurrentUser.Id
            };
        }

        private SetPaymentPassword BuildSetPaymentPasswordCommand(SetPaymentPasswordRequest request)
        {
            return new SetPaymentPassword
            {
                Args = this.BuildArgs(),
                Override = false,
                PaymentPassword = request.Password,
                Salt = this.CurrentUser.Id.ToGuidString(),
                UserId = this.CurrentUser.Id
            };
        }

        private UserRegister BuildUserRegisterCommand(SignUpRequest request, SignUpUserIdInfo info)
        {
            return new UserRegister
            {
                EntityId = info.UserId,
                Args = this.BuildArgs(),
                Cellphone = info.Cellphone,
                ClientType = request.ClientType.GetValueOrDefault(),
                ContractId = request.ContractId.GetValueOrDefault(),
                InviteBy = request.InviteBy ?? "JYM",
                OutletCode = request.OutletCode ?? "JYM",
                Password = request.Password,
                UserId = info.UserId
            };
        }

        /// <summary>
        ///     Sets the cookie.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cellphone">The cellphone.</param>
        private void SetCookie(Guid userId, string cellphone)
        {
            bool isMobileDevice = HttpUtils.IsFromMobileDevice(this.Request);
            DateTime expiry = isMobileDevice ? DateTime.UtcNow.AddDays(7) : DateTime.UtcNow.AddMinutes(30);
            string userData = $"{userId},{cellphone},{expiry.ToBinary()}";
            FormsAuthentication.SetAuthCookie(userData, true);
            HttpCookie cookie = FormsAuthentication.GetAuthCookie(userData, true);
            HttpContext.Current.Response.Headers.Add("X-JYM-AUTH", cookie.Value);
        }
    }
}