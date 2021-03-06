﻿// ***********************************************************************
// Project          : io.yuyi.jinyinmao.server
// File             : IRegularProduct.cs
// Created          : 2015-08-13  15:17
//
// Last Modified By : Siqi Lu
// Last Modified On : 2015-08-13  23:39
// ***********************************************************************
// <copyright file="IRegularProduct.cs" company="Shanghai Yuyi Mdt InfoTech Ltd.">
//     Copyright ©  2012-2015 Shanghai Yuyi Mdt InfoTech Ltd. All rights reserved.
// </copyright>
// ***********************************************************************

using System.Threading.Tasks;
using Orleans;
using Yuyi.Jinyinmao.Domain.Commands;
using Yuyi.Jinyinmao.Domain.Dtos;

namespace Yuyi.Jinyinmao.Domain
{
    /// <summary>
    ///     Interface IRegularProduct
    /// </summary>
    public interface IRegularProduct : IGrainWithGuidKey
    {
        /// <summary>
        ///     Builds the order asynchronous.
        /// </summary>
        /// <returns>Task&lt;OrderInfo&gt;.</returns>
        Task<OrderInfo> BuildOrderAsync(OrderInfo orderInfo);

        /// <summary>
        ///     Checks the sale status asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        Task CheckSaleStatusAsync();

        /// <summary>
        ///     Dumps the asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        Task DumpAsync();

        /// <summary>
        ///     Gets the agreement asynchronous.
        /// </summary>
        /// <param name="agreementIndex">Index of the agreement.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        Task<string> GetAgreementAsync(int agreementIndex);

        /// <summary>
        ///     Gets the product paid amount asynchronous.
        /// </summary>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        Task<long> GetProductPaidAmountAsync();

        /// <summary>
        ///     Gets the regular product information asynchronous.
        /// </summary>
        /// <returns>Task&lt;RegularProductInfo&gt;.</returns>
        Task<RegularProductInfo> GetRegularProductInfoAsync();

        /// <summary>
        ///     Hits the shelves.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>Task.</returns>
        Task<RegularProductInfo> HitShelvesAsync(IssueRegularProduct command);

        /// <summary>
        ///     Migrates the asynchronous.
        /// </summary>
        /// <param name="migrationDto">The migration dto.</param>
        /// <returns>Task.</returns>
        Task<RegularProductInfo> MigrateAsync(RegularProductMigrationDto migrationDto);

        /// <summary>
        ///     Reloads the asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        Task<RegularProductInfo> ReloadAsync();

        /// <summary>
        ///     Repays the asynchronous.
        /// </summary>
        /// <param name="productRepayCommand">The product repay command.</param>
        /// <returns></returns>
        Task<RegularProductInfo> RepayAsync(ProductRepay productRepayCommand);

        /// <summary>
        ///     Sets to on sale asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        Task SetToOnSaleAsync();

        /// <summary>
        ///     Sets to sold out asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        Task SetToSoldOutAsync();

        /// <summary>
        ///     Synchronizes the asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        Task SyncAsync();
    }
}