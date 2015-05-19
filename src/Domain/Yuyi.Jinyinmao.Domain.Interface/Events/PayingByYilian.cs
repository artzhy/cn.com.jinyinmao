// ***********************************************************************
// Project          : io.yuyi.jinyinmao.server
// Author           : Siqi Lu
// Created          : 2015-05-15  8:00 PM
//
// Last Modified By : Siqi Lu
// Last Modified On : 2015-05-15  8:12 PM
// ***********************************************************************
// <copyright file="PayingByYilian.cs" company="Shanghai Yuyi Mdt InfoTech Ltd.">
//     Copyright ©  2012-2015 Shanghai Yuyi Mdt InfoTech Ltd. All rights reserved.
// </copyright>
// ***********************************************************************

using Orleans.Concurrency;
using Yuyi.Jinyinmao.Domain.Dtos;

namespace Yuyi.Jinyinmao.Domain.Events
{
    /// <summary>
    ///     PayingByYilian.
    /// </summary>
    [Immutable]
    public class PayingByYilian : Event
    {
        /// <summary>
        ///     Gets or sets the transcation information.
        /// </summary>
        /// <value>The transcation information.</value>
        public SettleAccountTranscationInfo TranscationInfo { get; set; }

        /// <summary>
        ///     Gets or sets the user information.
        /// </summary>
        /// <value>The user information.</value>
        public UserInfo UserInfo { get; set; }
    }
}