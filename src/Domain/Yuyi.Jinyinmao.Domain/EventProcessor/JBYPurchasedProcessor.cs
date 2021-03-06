// ***********************************************************************
// Project          : io.yuyi.jinyinmao.server
// Author           : Siqi Lu
// Created          : 2015-05-27  7:39 PM
//
// Last Modified By : Siqi Lu
// Last Modified On : 2015-06-30  1:19 AM
// ***********************************************************************
// <copyright file="JBYPurchasedProcessor.cs" company="Shanghai Yuyi Mdt InfoTech Ltd.">
//     Copyright ©  2012-2015 Shanghai Yuyi Mdt InfoTech Ltd. All rights reserved.
// </copyright>
// ***********************************************************************

using System;
using System.Threading.Tasks;
using Moe.Lib;

namespace Yuyi.Jinyinmao.Domain.Events
{
    /// <summary>
    ///     JBYPurchasedProcessor.
    /// </summary>
    public class JBYPurchasedProcessor : EventProcessor<JBYPurchased>, IJBYPurchasedProcessor
    {
        #region IJBYPurchasedProcessor Members

        /// <summary>
        ///     Processes the event.
        /// </summary>
        /// <param name="event">The event.</param>
        /// <returns>Task.</returns>
        public override async Task ProcessEventAsync(JBYPurchased @event)
        {
            await this.ProcessingEventAsync(@event, async e =>
            {
                string message = Resources.Sms_JBYPurchased.FormatWith(e.JBYTransactionInfo.Amount / 100);
                if (!await this.SmsService.SendMessageAsync(e.UserInfo.Cellphone, message))
                {
                    throw new ApplicationException("Sms sending failed. {0}-{1}".FormatWith(e.UserInfo.Cellphone, message));
                }
            });

            await this.ProcessingEventAsync(@event, async e =>
            {
                await DBSyncHelper.SyncJBYAccountTransactionAsync(e.JBYTransactionInfo);
                await DBSyncHelper.SyncSettleAccountTransactionAsync(e.SettleTransactionInfo);
            });

            await base.ProcessEventAsync(@event);
        }

        #endregion IJBYPurchasedProcessor Members
    }
}