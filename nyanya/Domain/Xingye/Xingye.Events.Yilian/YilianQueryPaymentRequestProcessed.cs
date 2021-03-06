﻿// FileInformation: nyanya/Xingye.Events.Yilian/YilianQueryPaymentRequestProcessed.cs
// CreatedTime: 2014/09/01   2:44 PM
// LastUpdatedTime: 2014/09/01   4:27 PM

using System;
using Domian.Events;
using ServiceStack.FluentValidation;

namespace Xingye.Events.Yilian
{
    public class YilianQueryPaymentRequestProcessed : Event, IYilianPaymentResultEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="YilianAuthRequestSended" /> class.
        ///     Only for Serialization
        /// </summary>
        public YilianQueryPaymentRequestProcessed()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <c>Event</c> class.
        /// </summary>
        /// <param name="reqeustIdentifier">The reqeust identifier.</param>
        /// <param name="sourceType">Type of the source.</param>
        public YilianQueryPaymentRequestProcessed(string reqeustIdentifier, Type sourceType)
            : base(reqeustIdentifier, sourceType)
        {
            this.RequestIdentifier = reqeustIdentifier;
        }

        public string RequestIdentifier { get; set; }

        #region IYilianPaymentResultEvent Members

        public string Message { get; set; }

        public string OrderIdentifier { get; set; }

        public bool Result { get; set; }

        public string SequenceNo { get; set; }

        public string UserIdentifier { get; set; }

        #endregion IYilianPaymentResultEvent Members
    }

    public class YilianQueryPaymentRequestProcessedValidator : AbstractValidator<YilianQueryPaymentRequestProcessed>
    {
        public YilianQueryPaymentRequestProcessedValidator()
        {
            this.RuleFor(c => c.RequestIdentifier).NotNull();
            this.RuleFor(c => c.RequestIdentifier).NotEmpty();

            this.RuleFor(c => c.SequenceNo).NotNull();
            this.RuleFor(c => c.SequenceNo).NotEmpty();

            this.RuleFor(c => c.UserIdentifier).NotNull();
            this.RuleFor(c => c.UserIdentifier).NotEmpty();

            this.RuleFor(c => c.OrderIdentifier).NotNull();
            this.RuleFor(c => c.OrderIdentifier).NotEmpty();
        }
    }
}