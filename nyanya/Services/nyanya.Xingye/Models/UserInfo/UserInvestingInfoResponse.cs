﻿// FileInformation: nyanya/nyanya.Xingye/UserInvestingInfoResponse.cs
// CreatedTime: 2014/09/01   10:17 AM
// LastUpdatedTime: 2014/09/02   3:28 PM

using System;
using Infrastructure.Lib.Extensions;
using Xingye.Domain.Orders.Services.DTO;

namespace nyanya.Xingye.Models
{
    /// <summary>
    ///     UserInvestingInfoResponse
    /// </summary>
    public class UserInvestingInfoResponse
    {
        #region Public Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="UserInvestingInfoResponse" /> class.
        /// </summary>
        public UserInvestingInfoResponse()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="UserInvestingInfoResponse" /> class.
        /// </summary>
        /// <param name="maxIncomeSpeed">The maximum income speed.</param>
        /// <param name="userIncomeSpeed">The user income speed.</param>
        /// <param name="investingInfo">The investing information.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public UserInvestingInfoResponse(decimal maxIncomeSpeed, decimal userIncomeSpeed, InvestingInfo investingInfo)
        {
            this.ExpectedInterest = decimal.Round(investingInfo.Interest, 2, MidpointRounding.AwayFromZero);
            this.IncomePerMinute = decimal.Round((userIncomeSpeed / (360 * 24 * 60)), 10);
            this.InvestingPrincipal = investingInfo.Principal.ToIntFormat();
            this.TotalInterest = decimal.Round(investingInfo.TotalInterest, 2, MidpointRounding.AwayFromZero);
            this.DefeatedPercent = (userIncomeSpeed > maxIncomeSpeed) ? 99
                : decimal.ToInt32((Convert.ToDecimal(Math.Sqrt(Convert.ToDouble(userIncomeSpeed / maxIncomeSpeed))) * 100));
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        ///     打败的百分比
        /// </summary>
        public int DefeatedPercent { get; set; }

        /// <summary>
        ///     预期收益
        /// </summary>
        public decimal ExpectedInterest { get; set; }

        /// <summary>
        ///     每分钟赚取的收益
        /// </summary>
        public decimal IncomePerMinute { get; set; }

        /// <summary>
        ///     在投金额
        /// </summary>
        public decimal InvestingPrincipal { get; set; }

        /// <summary>
        ///     已获收益
        /// </summary>
        public decimal TotalInterest { get; set; }

        #endregion Public Properties
    }
}