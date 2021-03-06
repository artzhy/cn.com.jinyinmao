﻿// FileInformation: nyanya/Cat.Events.Users/AppliedForSignUpPayment.cs
// CreatedTime: 2014/08/10   1:23 PM
// LastUpdatedTime: 2014/08/12   11:33 AM

using System;
using Domian.Events;
using Infrastructure.Lib.Utility;
using ServiceStack.FluentValidation;

namespace Cat.Events.Users
{
    /// <summary>
    ///     申请注册支付
    /// </summary>
    public class AppliedForSignUpPayment : Event, IAppliedForYilianAuth
    {
        /// <summary>
        ///     初始化<see cref="AppliedForSignUpPayment" />类的新实例. 
        /// </summary>
        public AppliedForSignUpPayment()
        {
        }
        /// <summary>
        ///     初始化<see cref="AppliedForSignUpPayment" />类的新实例. 
        /// </summary>
        /// <param name="userIdentifier">用户标识</param>
        /// <param name="sourceType">源类型</param>
        public AppliedForSignUpPayment(string userIdentifier, Type sourceType)
            : base(userIdentifier, sourceType)
        {
            this.UserIdentifier = userIdentifier;
        }

        #region IAppliedForYilianAuth Members
        /// <summary>
        ///     金额（1.08）
        /// </summary>
        public decimal Amount
        {
            get { return new decimal(1.08); }
        }
        /// <summary>
        ///     银行卡号
        /// </summary>
        public string BankCardNo { get; set; }
        /// <summary>
        ///     银行名称
        /// </summary>
        public string BankName { get; set; }
        /// <summary>
        ///     手机号
        /// </summary>
        public string Cellphone { get; set; }
        /// <summary>
        ///     银行开户城市名
        /// </summary>
        public string CityName { get; set; }
        /// <summary>
        ///     证件类型
        /// </summary>
        public int CredentialCode { get; set; }
        /// <summary>
        ///     证件号
        /// </summary>
        public string CredentialNo { get; set; }
        /// <summary>
        ///     订单标识
        /// </summary>
        public string OrderIdentifier
        {
            get { return ""; }
        }
        /// <summary>
        ///     产品标识
        /// </summary>
        public string ProductIdentifier
        {
            get { return ""; }
        }
        /// <summary>
        ///     产品编号
        /// </summary>
        public string ProductNo
        {
            get { return ""; }
        }
        /// <summary>
        ///     真实姓名
        /// </summary>
        public string RealName { get; set; }
        /// <summary>
        ///     订单编号
        /// </summary>
        public string SequenceNo { get; set; }
        /// <summary>
        ///     用户标识
        /// </summary>
        public string UserIdentifier { get; set; }

        #endregion IAppliedForYilianAuth Members
    }
    /// <summary>
    ///     申请注册支付（验证）
    /// </summary>
    public class AppliedForSignUpPaymentValidator : AbstractValidator<AppliedForSignUpPayment>
    {
        /// <summary>
        ///     初始化<see cref="AppliedForSignUpPaymentValidator" />类的新实例. 
        /// </summary>
        public AppliedForSignUpPaymentValidator()
        {
            this.RuleFor(c => c.BankCardNo).NotNull();
            this.RuleFor(c => c.BankCardNo).NotEmpty();

            this.RuleFor(c => c.BankName).NotNull();
            this.RuleFor(c => c.BankName).NotEmpty();

            this.RuleFor(c => c.Cellphone).NotNull();
            this.RuleFor(c => c.Cellphone).NotEmpty();
            this.RuleFor(c => c.Cellphone).Matches(RegexUtils.CellphoneRegex.ToString());

            this.RuleFor(c => c.CityName).NotNull();
            this.RuleFor(c => c.CityName).NotEmpty();

            this.RuleFor(c => c.RealName).NotNull();
            this.RuleFor(c => c.RealName).NotEmpty();

            this.RuleFor(c => c.CredentialNo).NotNull();
            this.RuleFor(c => c.CredentialNo).NotEmpty();

            this.RuleFor(c => c.UserIdentifier).NotNull();
            this.RuleFor(c => c.UserIdentifier).NotEmpty();

            this.RuleFor(c => c.SequenceNo).NotNull();
            this.RuleFor(c => c.SequenceNo).NotEmpty();
            this.RuleFor(c => c.SequenceNo).Length(14);
        }
    }
}