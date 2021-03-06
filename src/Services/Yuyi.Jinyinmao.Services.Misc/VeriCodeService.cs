// ***********************************************************************
// Project          : io.yuyi.jinyinmao.server
// Author           : Siqi Lu
// Created          : 2015-04-19  5:34 PM
//
// Last Modified By : Siqi Lu
// Last Modified On : 2015-07-15  12:43 PM
// ***********************************************************************
// <copyright file="VeriCodeService.cs" company="Shanghai Yuyi Mdt InfoTech Ltd.">
//     Copyright ©  2012-2015 Shanghai Yuyi Mdt InfoTech Ltd. All rights reserved.
// </copyright>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Moe.Lib;
using Yuyi.Jinyinmao.Service.Interface;
using Yuyi.Jinyinmao.Service.Models;

namespace Yuyi.Jinyinmao.Service
{
    /// <summary>
    ///     Class VeriCodeService.
    /// </summary>
    public class VeriCodeService : IVeriCodeService
    {
        /// <summary>
        ///     The maximum send times
        /// </summary>
        private static readonly int MaxSendTimes = 10;

        /// <summary>
        ///     The veri code validity in minute
        /// </summary>
        private static readonly int VeriCodeValidityInMinute = 30;

        /// <summary>
        ///     The SMS service
        /// </summary>
        private readonly ISmsService smsService;

        /// <summary>
        ///     Initializes a new instance of the <see cref="VeriCodeService" /> class.
        /// </summary>
        /// <param name="smsService">The SMS service.</param>
        public VeriCodeService(ISmsService smsService)
        {
            this.smsService = smsService;
        }

        #region IVeriCodeService Members

        /// <summary>
        ///     Sends the asynchronous.
        /// </summary>
        /// <param name="cellphone">The cellphone.</param>
        /// <param name="type">The type.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>Task&lt;SendVeriCodeResult&gt;.</returns>
        public async Task<SendVeriCodeResult> SendAsync(string cellphone, VeriCodeType type, Dictionary<string, object> args)
        {
            string veriCode;
            VeriCode code;

            using (JYMDBContext context = new JYMDBContext())
            {
                // 时间大于今天开始日期，就一定是今天发送的验证码
                code = await context.Query<VeriCode>().OrderByDescending(c => c.BuildAt)
                    .FirstOrDefaultAsync(c => c.Cellphone == cellphone && c.Type == (int)type && c.BuildAt >= DateTime.Today);

                // 超过最大次数，停止发送
                if (code != null && code.Times >= MaxSendTimes)
                {
                    return new SendVeriCodeResult { RemainCount = -1, Success = false };
                }

                veriCode = GenerateCode();

                // 小于最大次数，再次发送
                if (code != null && code.Times < MaxSendTimes)
                {
                    // 增加失败次数
                    code.Times += 1;
                    // 重新生成验证码
                    code.Code += "|" + veriCode;
                    // 重置验证码验证数据
                    code.ErrorCount = 0;
                    code.Verified = false;
                    code.Used = false;
                    code.BuildAt = DateTime.UtcNow.ToChinaStandardTime();
                }

                // 没有记录，重新生成
                if (code == null)
                {
                    code = new VeriCode
                    {
                        Cellphone = cellphone,
                        Token = Guid.NewGuid().ToGuidString(),
                        Code = veriCode,
                        ErrorCount = 0,
                        BuildAt = DateTime.UtcNow.ToChinaStandardTime(),
                        Times = 1,
                        Type = (int)type,
                        Used = false,
                        Verified = false,
                        Args = args.ToJson()
                    };

                    context.Add(code);
                }

                await context.ExecuteSaveChangesAsync();
            }

            string message = GetVeriCodeMessage(type);
            string verifyMessage = message.FormatWith(veriCode, VeriCodeValidityInMinute);
            await this.smsService.SendMessageAsync(cellphone, verifyMessage);

            return new SendVeriCodeResult { RemainCount = MaxSendTimes - code.Times, Success = true };
        }

        /// <summary>
        ///     Uses the asynchronous.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="type">The type.</param>
        /// <returns>Task&lt;UseVeriCodeResult&gt;.</returns>
        public async Task<UseVeriCodeResult> UseAsync(string token, VeriCodeType type)
        {
            using (JYMDBContext context = new JYMDBContext())
            {
                // 验证码的使用有效期为30分钟
                DateTime availableTime = DateTime.UtcNow.ToChinaStandardTime().AddMinutes(-VeriCodeValidityInMinute);
                VeriCode veriCode = await context.Query<VeriCode>().OrderByDescending(v => v.BuildAt)
                    .FirstOrDefaultAsync(v => v.Token == token && v.Type == (int)type && v.BuildAt >= availableTime);

                if (veriCode == null || !veriCode.Verified || veriCode.Used) return new UseVeriCodeResult();

                veriCode.Used = true;
                await context.ExecuteSaveChangesAsync();

                return new UseVeriCodeResult
                {
                    Cellphone = veriCode.Cellphone,
                    Result = true
                };
            }
        }

        /// <summary>
        ///     Verifies the asynchronous.
        /// </summary>
        /// <param name="cellphone">The cellphone.</param>
        /// <param name="code">The code.</param>
        /// <param name="type">The type.</param>
        /// <returns>Task&lt;VerifyVeriCodeResult&gt;.</returns>
        public async Task<VerifyVeriCodeResult> VerifyAsync(string cellphone, string code, VeriCodeType type)
        {
            using (JYMDBContext context = new JYMDBContext())
            {
                // 只取有效期内的验证码
                DateTime availableTime = DateTime.UtcNow.ToChinaStandardTime().AddMinutes(-VeriCodeValidityInMinute);
                VeriCode veriCode = await context.Query<VeriCode>().OrderByDescending(v => v.BuildAt)
                    .FirstOrDefaultAsync(v => v.Cellphone == cellphone && v.Type == (int)type && v.BuildAt >= availableTime && !v.Used);

                // 无该手机验证码记录，或者超过3次，验证码失效
                if (veriCode == null || veriCode.ErrorCount >= 3)
                {
                    return new VerifyVeriCodeResult { RemainCount = -1, Success = false };
                }

                // 少于3次，执行验证
                if (veriCode.Code.Split('|').Contains(code))
                {
                    if (!veriCode.Verified)
                    {
                        veriCode.Verified = true;
                        await context.SaveChangesAsync();
                    }
                    // 验证成功，返回token
                    return new VerifyVeriCodeResult { Success = true, Token = veriCode.Token };
                }

                // 验证未通过，且失败次数少于2次，递增失败次数
                veriCode.ErrorCount += 1;
                veriCode.Verified = false;
                await context.SaveChangesAsync();
                return new VerifyVeriCodeResult { Success = false, RemainCount = 3 - veriCode.ErrorCount };
            }
        }

        #endregion IVeriCodeService Members

        private static string GenerateCode()
        {
            Random r = new Random();
            return r.Next(100000, 999999).ToString();
        }

        private static string GetVeriCodeMessage(VeriCodeType type)
        {
            switch (type)
            {
                case VeriCodeType.SignUp:
                    return Resources.Sms_VeriCode_SignUp;

                case VeriCodeType.ResetLoginPassword:
                    return Resources.Sms_VeriCode_ResetLoginPawword;

                case VeriCodeType.ResetPaymentPassword:
                    return Resources.Sms_VeriCode_ResetPaymentPawword;

                default:
                    return Resources.Sms_VeriCode;
            }
        }
    }
}