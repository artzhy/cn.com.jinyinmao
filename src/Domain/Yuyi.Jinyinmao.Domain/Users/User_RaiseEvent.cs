// ***********************************************************************
// Project          : io.yuyi.jinyinmao.server
// File             : User_RaiseEvent.cs
// Created          : 2015-05-27  7:39 PM
//
// Last Modified By : Siqi Lu
// Last Modified On : 2015-08-03  11:35 AM
// ***********************************************************************
// <copyright file="User_RaiseEvent.cs" company="Shanghai Yuyi Mdt InfoTech Ltd.">
//     Copyright ©  2012-2015 Shanghai Yuyi Mdt InfoTech Ltd. All rights reserved.
// </copyright>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moe.Lib;
using Yuyi.Jinyinmao.Domain.Commands;
using Yuyi.Jinyinmao.Domain.Dtos;
using Yuyi.Jinyinmao.Domain.Events;

namespace Yuyi.Jinyinmao.Domain
{
    /// <summary>
    ///     User.
    /// </summary>
    public partial class User
    {
        /// <summary>
        ///     The event processing
        /// </summary>
        private static readonly Dictionary<Type, Func<IEvent, Task>> EventProcessing = new Dictionary<Type, Func<IEvent, Task>>
        {
            { typeof(BankCardAdded), e => BankCardAddedProcessorFactory.GetGrain(e.EventId).ProcessEventAsync((BankCardAdded)e) },
            { typeof(AuthenticateResulted), e => AuthenticateResultedProcessorFactory.GetGrain(e.EventId).ProcessEventAsync((AuthenticateResulted)e) },
            { typeof(VerifyBankCardResulted), e => VerifyBankCardResultedProcessorFactory.GetGrain(e.EventId).ProcessEventAsync((VerifyBankCardResulted)e) },
            { typeof(PayingByYilian), e => PayingByYilianProcessorFactory.GetGrain(e.EventId).ProcessEventAsync((PayingByYilian)e) },
            { typeof(DepositResulted), e => DepositResultedProcessorFactory.GetGrain(e.EventId).ProcessEventAsync((DepositResulted)e) },
            { typeof(JBYPurchased), e => JBYPurchasedProcessorFactory.GetGrain(e.EventId).ProcessEventAsync((JBYPurchased)e) },
            { typeof(OrderPaid), e => OrderPaidProcessorFactory.GetGrain(e.EventId).ProcessEventAsync((OrderPaid)e) },
            { typeof(UserRegistered), e => UserRegisteredProcessorFactory.GetGrain(e.EventId).ProcessEventAsync((UserRegistered)e) },
            { typeof(OrderRepaid), e => OrderRepaidProcessorFactory.GetGrain(e.EventId).ProcessEventAsync((OrderRepaid)e) },
            { typeof(LoginPasswordReset), e => LoginPasswordResetProcessorFactory.GetGrain(e.EventId).ProcessEventAsync((LoginPasswordReset)e) },
            { typeof(PaymentPasswordReset), e => PaymentPasswordResetProcessorFactory.GetGrain(e.EventId).ProcessEventAsync((PaymentPasswordReset)e) },
            { typeof(PaymentPasswordSet), e => PaymentPasswordSetProcessorFactory.GetGrain(e.EventId).ProcessEventAsync((PaymentPasswordSet)e) },
            { typeof(WithdrawalAccepted), e => WithdrawalAcceptedProcessorFactory.GetGrain(e.EventId).ProcessEventAsync((WithdrawalAccepted)e) },
            { typeof(WithdrawalResulted), e => WithdrawalResultedProcessorFactory.GetGrain(e.EventId).ProcessEventAsync((WithdrawalResulted)e) },
            { typeof(JBYWithdrawalAccepted), e => JBYWithdrawalAcceptedProcessorFactory.GetGrain(e.EventId).ProcessEventAsync((JBYWithdrawalAccepted)e) },
            { typeof(JBYWithdrawalResulted), e => JBYWithdrawalResultedProcessorFactory.GetGrain(e.EventId).ProcessEventAsync((JBYWithdrawalResulted)e) },
            { typeof(JBYReinvested), e => JBYReinvestedProcessorFactory.GetGrain(e.EventId).ProcessEventAsync((JBYReinvested)e) },
            { typeof(BankCardHiden), e => BankCardHidenProcessorFactory.GetGrain(e.EventId).ProcessEventAsync((BankCardHiden)e) },
            { typeof(ExtraInterestAdded), e => ExtraInterestAddedProcessorFactory.GetGrain(e.EventId).ProcessEventAsync((ExtraInterestAdded)e) },
            { typeof(SettleAccountTransactionInserted), e => SettleAccountTransactionInsertedProcessorFactory.GetGrain(e.EventId).ProcessEventAsync((SettleAccountTransactionInserted)e) },
            { typeof(JBYAccountTransactionInserted), e => JBYAccountTransactionInsertedProcessorFactory.GetGrain(e.EventId).ProcessEventAsync((JBYAccountTransactionInserted)e) }
        };

        /// <summary>
        ///     Raises the bank card added event.
        /// </summary>
        /// <param name="addBankCardCommand">The add bank card command.</param>
        /// <param name="bankCardInfo">The bank card information.</param>
        /// <returns>Task.</returns>
        public async Task RaiseBankCardAddedEvent(AddBankCard addBankCardCommand, BankCardInfo bankCardInfo)
        {
            BankCardAdded @event = new BankCardAdded
            {
                Args = addBankCardCommand.Args,
                BankCardInfo = bankCardInfo,
                UserInfo = await this.GetUserInfoAsync()
            };

            await this.ProcessEventAsync(@event);
        }

        /// <summary>
        ///     Stores the event asynchronous.
        /// </summary>
        /// <param name="event">The event.</param>
        /// <returns>Task.</returns>
        private async Task ProcessEventAsync(Event @event)
        {
            @event.SourceId = this.State.UserId.ToGuidString();
            @event.SourceType = this.GetType().Name;
            @event.TimeStamp = DateTime.UtcNow;

            this.StoreEventAsync(@event);

            await EventProcessing[@event.GetType()].Invoke(@event);
        }

        /// <summary>
        ///     Raises the authentication resulted event.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="bankCardInfo">The bank card information.</param>
        /// <param name="result">if set to <c>true</c> [result].</param>
        /// <param name="message">The message.</param>
        /// <returns>Task.</returns>
        private async Task RaiseAuthenticationResultedEvent(Authenticate command, BankCardInfo bankCardInfo, bool result, string message)
        {
            AuthenticateResulted @event = new AuthenticateResulted
            {
                Args = command.Args,
                BankCardInfo = bankCardInfo,
                Result = result,
                TranDesc = message,
                UserInfo = await this.GetUserInfoAsync()
            };

            await this.ProcessEventAsync(@event);
        }

        /// <summary>
        ///     Raises the bank card hiden event.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="bankCardInfo">The bank card information.</param>
        /// <returns>Task.</returns>
        private async Task RaiseBankCardHidenEvent(HideBankCard command, BankCardInfo bankCardInfo)
        {
            BankCardHiden @event = new BankCardHiden
            {
                Args = command.Args,
                BankCardInfo = bankCardInfo,
                UserInfo = await this.GetUserInfoAsync()
            };

            await this.ProcessEventAsync(@event);
        }

        private async Task RaiseDepositResultedEvent(Command command, SettleAccountTransactionInfo info, bool result, string message)
        {
            DepositResulted @event = new DepositResulted
            {
                Args = command.Args,
                Result = result,
                TransDesc = message,
                TransactionInfo = info,
                UserInfo = await this.GetUserInfoAsync()
            };

            await this.ProcessEventAsync(@event);
        }

        private async Task RaiseDepositResultedEvent(Dictionary<string, object> args, SettleAccountTransactionInfo info, bool result, string message)
        {
            DepositResulted @event = new DepositResulted
            {
                Args = args,
                Result = result,
                TransDesc = message,
                TransactionInfo = info,
                UserInfo = await this.GetUserInfoAsync()
            };

            await this.ProcessEventAsync(@event);
        }

        /// <summary>
        ///     Raises the extra interest added event.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="info">The information.</param>
        /// <param name="amount">The amount.</param>
        /// <returns>Task.</returns>
        private async Task RaiseExtraInterestAddedEvent(AddExtraInterest command, OrderInfo info, int amount)
        {
            ExtraInterestAdded @event = new ExtraInterestAdded
            {
                Amount = amount,
                Args = command.Args,
                Description = command.Description,
                ExtraInterest = command.ExtraInterest,
                ExtraPrincipal = command.ExtraPrincipal,
                OrderInfo = info
            };

            await this.ProcessEventAsync(@event);
        }

        private async Task RaiseJBYPurchasedEvent(JBYInvesting command, JBYAccountTransactionInfo jbyTransaction, SettleAccountTransactionInfo settleTransaction)
        {
            JBYPurchased @event = new JBYPurchased
            {
                Args = command.Args,
                JBYTransactionInfo = jbyTransaction,
                SettleTransactionInfo = settleTransaction,
                UserInfo = await this.GetUserInfoAsync()
            };

            await this.ProcessEventAsync(@event);
        }

        private async Task RaiseJBYReinvestedEvent(JBYAccountTransactionInfo jbyAccountTransactionInfo)
        {
            JBYReinvested @event = new JBYReinvested
            {
                Args = new Dictionary<string, object>(),
                TransactionInfo = jbyAccountTransactionInfo,
                UserInfo = await this.GetUserInfoAsync()
            };

            await this.ProcessEventAsync(@event);
        }

        private async Task RaiseJBYWithdrawalAcceptedEvent(JBYWithdrawal command, JBYAccountTransactionInfo info)
        {
            JBYWithdrawalAccepted @event = new JBYWithdrawalAccepted
            {
                Args = command.Args,
                TransactionInfo = info,
                UserInfo = await this.GetUserInfoAsync()
            };

            await this.ProcessEventAsync(@event);
        }

        private async Task RaiseJBYWithdrawalResultedEvent(JBYAccountTransactionInfo jbyAccountTransactionInfo, SettleAccountTransactionInfo settleAccountTransactionInfo)
        {
            JBYWithdrawalResulted @event = new JBYWithdrawalResulted
            {
                Args = new Dictionary<string, object>(),
                JBYAccountTransactionInfo = jbyAccountTransactionInfo,
                SettleAccountTransactionInfo = settleAccountTransactionInfo,
                UserInfo = await this.GetUserInfoAsync()
            };

            await this.ProcessEventAsync(@event);
        }

        /// <summary>
        ///     Raises the login password reset event.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>Task.</returns>
        private async Task RaiseLoginPasswordResetEvent(ResetLoginPassword command)
        {
            LoginPasswordReset @event = new LoginPasswordReset
            {
                Args = command.Args,
                UserInfo = await this.GetUserInfoAsync()
            };

            await this.ProcessEventAsync(@event);
        }

        /// <summary>
        ///     Raises the order built event.
        /// </summary>
        /// <param name="orderInfo">The order information.</param>
        /// <param name="transactionInfo">The transaction information.</param>
        /// <returns>Task.</returns>
        private async Task RaiseOrderPaidEvent(OrderInfo orderInfo, SettleAccountTransactionInfo transactionInfo)
        {
            OrderPaid @event = new OrderPaid
            {
                Args = new Dictionary<string, object>(),
                OrderInfo = orderInfo,
                TransactionInfo = transactionInfo
            };

            await this.ProcessEventAsync(@event);
        }

        /// <summary>
        ///     Raises the order repaid event.
        /// </summary>
        /// <param name="orderInfo">The order information.</param>
        /// <param name="principalTransactionInfo">The principal transaction information.</param>
        /// <param name="interestTransactionInfo">The interest transaction information.</param>
        private async void RaiseOrderRepaidEvent(OrderInfo orderInfo, SettleAccountTransactionInfo principalTransactionInfo, SettleAccountTransactionInfo interestTransactionInfo)
        {
            OrderRepaid @event = new OrderRepaid
            {
                Args = new Dictionary<string, object>(),
                InterestTransactionInfo = interestTransactionInfo,
                OrderInfo = orderInfo,
                PriIntSumAmount = principalTransactionInfo.Amount + interestTransactionInfo.Amount,
                PrincipalTransactionInfo = principalTransactionInfo,
                RepaidTime = orderInfo.ResultTime.GetValueOrDefault(),
                UserInfo = await this.GetUserInfoAsync()
            };

            await this.ProcessEventAsync(@event);
        }

        /// <summary>
        ///     Raises the paying by yilian event.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="transactionInfo">The transaction information.</param>
        /// <returns>Task.</returns>
        private async Task RaisePayingByYilianEvent(Command command, SettleAccountTransactionInfo transactionInfo)
        {
            PayingByYilian @event = new PayingByYilian
            {
                Args = command.Args,
                TransactionInfo = transactionInfo,
                UserInfo = await this.GetUserInfoAsync()
            };

            await this.ProcessEventAsync(@event);
        }

        /// <summary>
        ///     Raises the payment password set event.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>Task.</returns>
        private async Task RaisePaymentPasswordSetEvent(SetPaymentPassword command)
        {
            Event @event;
            if (command.Override)
            {
                @event = new PaymentPasswordReset
                {
                    Args = command.Args,
                    UserInfo = await this.GetUserInfoAsync()
                };
            }
            else
            {
                @event = new PaymentPasswordSet
                {
                    Args = command.Args,
                    UserInfo = await this.GetUserInfoAsync()
                };
            }

            await this.ProcessEventAsync(@event);
        }

        private async Task RaiseTransactionInsertdEvent(InsertSettleAccountTransactionDto transactionDto, SettleAccountTransactionInfo transactionInfo)
        {
            SettleAccountTransactionInserted @event = new SettleAccountTransactionInserted
            {
                Args = transactionDto.Args,
                TransactionInfo = transactionInfo
            };

            await this.ProcessEventAsync(@event);
        }

        private async Task RaiseTransactionInsertdEvent(InsertJBYAccountTransactionDto transactionDto, JBYAccountTransactionInfo transactionInfo)
        {
            JBYAccountTransactionInserted @event = new JBYAccountTransactionInserted
            {
                Args = transactionDto.Args,
                TransactionInfo = transactionInfo
            };

            await this.ProcessEventAsync(@event);
        }

        /// <summary>
        ///     Raises the user registered event.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>Task.</returns>
        private async Task RaiseUserRegisteredEvent(UserRegister command)
        {
            UserRegistered @event = new UserRegistered
            {
                Args = command.Args,
                UserInfo = await this.GetUserInfoAsync()
            };

            await this.ProcessEventAsync(@event);
        }

        /// <summary>
        ///     Raises the verify bank card resulted event.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="bankCardInfo">The bank card information.</param>
        /// <param name="result">if set to <c>true</c> [result].</param>
        /// <param name="message">The message.</param>
        /// <returns>Task.</returns>
        private async Task RaiseVerifyBankCardResultedEvent(VerifyBankCard command, BankCardInfo bankCardInfo, bool result, string message)
        {
            VerifyBankCardResulted @event = new VerifyBankCardResulted
            {
                Args = command.Args,
                BankCardInfo = bankCardInfo,
                Result = result,
                TranDesc = message,
                UserInfo = await this.GetUserInfoAsync()
            };

            await this.ProcessEventAsync(@event);
        }

        private async Task RaiseWithdrawalAcceptedEvent(Withdrawal command, SettleAccountTransactionInfo transaction, SettleAccountTransactionInfo chargeTransaction)
        {
            WithdrawalAccepted @event = new WithdrawalAccepted
            {
                Args = command.Args,
                ChargeTransaction = chargeTransaction,
                UserInfo = await this.GetUserInfoAsync(),
                WithdrawalTransaction = transaction
            };

            await this.ProcessEventAsync(@event);
        }

        /// <summary>
        ///     Raises the withdrawal resulted event.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <returns>Task.</returns>
        private async Task RaiseWithdrawalResultedEvent(SettleAccountTransactionInfo transaction)
        {
            WithdrawalResulted @event = new WithdrawalResulted
            {
                Args = new Dictionary<string, object>(),
                UserInfo = await this.GetUserInfoAsync(),
                WithdrawalTransactionInfo = transaction
            };

            await this.ProcessEventAsync(@event);
        }
    }
}