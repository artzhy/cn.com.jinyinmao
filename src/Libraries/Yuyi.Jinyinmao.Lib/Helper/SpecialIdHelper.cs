// ***********************************************************************
// Project          : io.yuyi.jinyinmao.server
// File             : SpecialIdHelper.cs
// Created          : 2015-05-23  10:41 PM
//
// Last Modified By : Siqi Lu
// Last Modified On : 2015-08-03  11:19 AM
// ***********************************************************************
// <copyright file="SpecialIdHelper.cs" company="Shanghai Yuyi Mdt InfoTech Ltd.">
//     Copyright ©  2012-2015 Shanghai Yuyi Mdt InfoTech Ltd. All rights reserved.
// </copyright>
// ***********************************************************************

using System;

namespace Yuyi.Jinyinmao.Packages.Helper
{
    /// <summary>
    ///     SpecialIdHelper.
    /// </summary>
    public static class SpecialIdHelper
    {
        /// <summary>
        ///     Gets the j reinvesting by transaction product identifier.
        /// </summary>
        /// <value>The j reinvesting by transaction product identifier.</value>
        public static Guid ReinvestingJBYTransactionProductId { get; } = Guid.Parse("100693CD-890A-4193-9590-61BC1DDBD275");

        /// <summary>
        ///     Gets the reversal jby transaction product identifier.
        /// </summary>
        /// <value>The reversal jby transaction product identifier.</value>
        public static Guid ReversalJBYTransactionProductId { get; } = Guid.Parse("D6034A3B-2D74-4E5A-A676-CF7D3D67CB08");

        /// <summary>
        ///     Gets the withdrawal jby transaction product identifier.
        /// </summary>
        /// <value>The withdrawal jby transaction product identifier.</value>
        public static Guid WithdrawalJBYTransactionProductId { get; } = Guid.Parse("92CFADC4-91A5-4A09-8D0E-AC122C837F5B");
    }
}