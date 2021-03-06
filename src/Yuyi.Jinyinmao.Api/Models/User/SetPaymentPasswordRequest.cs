﻿// ***********************************************************************
// Project          : io.yuyi.jinyinmao.server
// Author           : Siqi Lu
// Created          : 2015-04-25  2:44 AM
//
// Last Modified By : Siqi Lu
// Last Modified On : 2015-04-28  10:06 AM
// ***********************************************************************
// <copyright file="SetPaymentPasswordRequest.cs" company="Shanghai Yuyi">
//     Copyright ©  2012-2015 Shanghai Yuyi. All rights reserved.
// </copyright>
// ***********************************************************************

using System.ComponentModel.DataAnnotations;
using Moe.AspNet.Models;
using Newtonsoft.Json;
using Yuyi.Jinyinmao.Api.Validations;

namespace Yuyi.Jinyinmao.Api.Models
{
    /// <summary>
    ///     SetPaymentPasswordRequest.
    /// </summary>
    public class SetPaymentPasswordRequest : IRequest
    {
        /// <summary>
        ///     支付密码
        /// </summary>
        [Required, StringLength(18, MinimumLength = 8), PaymentPasswordFormat, JsonProperty("password")]
        public string Password { get; set; }
    }
}
