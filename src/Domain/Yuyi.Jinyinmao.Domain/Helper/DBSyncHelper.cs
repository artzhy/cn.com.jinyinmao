// ***********************************************************************
// Project          : io.yuyi.jinyinmao.server
// File             : DBSyncHelper.cs
// Created          : 2015-05-27  7:39 PM
//
// Last Modified By : Siqi Lu
// Last Modified On : 2015-08-06  12:13 AM
// ***********************************************************************
// <copyright file="DBSyncHelper.cs" company="Shanghai Yuyi Mdt InfoTech Ltd.">
//     Copyright ©  2012-2015 Shanghai Yuyi Mdt InfoTech Ltd. All rights reserved.
// </copyright>
// ***********************************************************************

using System.Data.Entity;
using System.Threading.Tasks;
using Moe.Lib;
using Yuyi.Jinyinmao.Domain.Dtos;
using Yuyi.Jinyinmao.Domain.Models;

namespace Yuyi.Jinyinmao.Domain
{
    /// <summary>
    ///     DBSyncHelper.
    /// </summary>
    internal static class DBSyncHelper
    {
        internal static async Task RemoveJBYAccountTransactionAsync(JBYAccountTransactionInfo info)
        {
            string transactionIdentifier = info.TransactionId.ToGuidString();
            using (JYMDBContext db = new JYMDBContext())
            {
                JBYTransaction transaction = await db.Query<JBYTransaction>()
                    .FirstOrDefaultAsync(t => t.TransactionIdentifier == transactionIdentifier);

                if (transaction != null)
                {
                    await db.RemoveAsync(transaction);
                }
            }
        }

        internal static async Task RemoveOrderAsync(OrderInfo info)
        {
            string orderIdentifier = info.OrderId.ToGuidString();
            using (JYMDBContext db = new JYMDBContext())
            {
                Models.Order order = await db.Query<Models.Order>()
                    .FirstOrDefaultAsync(o => o.OrderIdentifier == orderIdentifier);

                if (order != null)
                {
                    await db.RemoveAsync(order);
                }
            }
        }

        internal static async Task RemoveSettleAccountTransactionAsync(SettleAccountTransactionInfo info)
        {
            string transactionIdentifier = info.TransactionId.ToGuidString();
            using (JYMDBContext db = new JYMDBContext())
            {
                AccountTransaction transaction = await db.Query<AccountTransaction>()
                    .FirstOrDefaultAsync(t => t.TransactionIdentifier == transactionIdentifier);

                if (transaction != null)
                {
                    await db.RemoveAsync(transaction);
                }
            }
        }

        internal static async Task SyncBankCardAsync(BankCardInfo info, string userIdentifier)
        {
            using (JYMDBContext db = new JYMDBContext())
            {
                Models.BankCard card = await db.Query<Models.BankCard>().FirstOrDefaultAsync(c => c.BankCardNo == info.BankCardNo && c.UserIdentifier == userIdentifier);

                if (card == null)
                {
                    db.BankCards.Add(info.ToDBModel());
                }
                else
                {
                    info.MapToDBModel(card);
                }

                await db.SaveChangesAsync();
            }
        }

        internal static async Task SyncJBYAccountTransactionAsync(JBYAccountTransactionInfo info)
        {
            string transactionIdentifier = info.TransactionId.ToGuidString();
            using (JYMDBContext db = new JYMDBContext())
            {
                JBYTransaction transaction = await db.Query<JBYTransaction>()
                    .FirstOrDefaultAsync(t => t.TransactionIdentifier == transactionIdentifier);

                if (transaction == null)
                {
                    db.JBYTransactions.Add(info.ToDBModel());
                }
                else
                {
                    info.MapToDBModel(transaction);
                }

                await db.ExecuteSaveChangesAsync();
            }
        }

        internal static async Task SyncJBYProductAsync(JBYProductInfo info, params string[] agreements)
        {
            string productIdentifier = info.ProductId.ToGuidString();
            using (JYMDBContext db = new JYMDBContext())
            {
                JBYProduct product = await db.Query<JBYProduct>().FirstOrDefaultAsync(p => p.ProductIdentifier == productIdentifier);

                if (product == null)
                {
                    db.JBYProducts.Add(info.ToDBModel(agreements));
                }
                else
                {
                    info.MapToDBModel(product, agreements);
                }

                await db.SaveChangesAsync();
            }
        }

        internal static async Task SyncOrderAsync(OrderInfo info)
        {
            string orderIdentifier = info.OrderId.ToGuidString();
            using (JYMDBContext db = new JYMDBContext())
            {
                Models.Order order = await db.Query<Models.Order>()
                    .FirstOrDefaultAsync(t => t.OrderIdentifier == orderIdentifier);

                if (order == null)
                {
                    db.Orders.Add(info.ToDBModel());
                }
                else
                {
                    info.MapToDBModel(order);
                }

                await db.ExecuteSaveChangesAsync();
            }
        }

        internal static async Task SyncRegularProductAsync(RegularProductInfo info, params string[] agreements)
        {
            string productIdentifier = info.ProductId.ToGuidString();
            using (JYMDBContext db = new JYMDBContext())
            {
                Models.RegularProduct product = await db.Query<Models.RegularProduct>().FirstOrDefaultAsync(p => p.ProductIdentifier == productIdentifier);

                if (product == null)
                {
                    db.RegularProducts.Add(info.ToDBModel(agreements));
                }
                else
                {
                    info.MapToDBModel(product, agreements);
                }

                await db.SaveChangesAsync();
            }
        }

        internal static async Task SyncSettleAccountTransactionAsync(SettleAccountTransactionInfo info)
        {
            string transactionIdentifier = info.TransactionId.ToGuidString();
            using (JYMDBContext db = new JYMDBContext())
            {
                AccountTransaction transaction = await db.Query<AccountTransaction>()
                    .FirstOrDefaultAsync(t => t.TransactionIdentifier == transactionIdentifier);

                if (transaction == null)
                {
                    db.AccountTransactions.Add(info.ToDBModel());
                }
                else
                {
                    info.MapToDBModel(transaction);
                }

                await db.ExecuteSaveChangesAsync();
            }
        }

        internal static async Task SyncUserAsync(UserInfo info)
        {
            string userIdentifier = info.UserId.ToGuidString();
            using (JYMDBContext db = new JYMDBContext())
            {
                Models.User user = await db.Query<Models.User>().FirstOrDefaultAsync(u => u.UserIdentifier == userIdentifier);

                if (user == null)
                {
                    db.Users.Add(info.ToDBModel());
                }
                else
                {
                    info.MapToDBModel(user);
                }

                await db.SaveChangesAsync();
            }
        }
    }
}