// ***********************************************************************
// Project          : io.yuyi.jinyinmao.server
// File             : MemoryWork.cs
// Created          : 2015-08-02  7:06 AM
//
// Last Modified By : Siqi Lu
// Last Modified On : 2015-08-02  7:20 AM
// ***********************************************************************
// <copyright file="MemoryWork.cs" company="Shanghai Yuyi Mdt InfoTech Ltd.">
//     Copyright ©  2012-2015 Shanghai Yuyi Mdt InfoTech Ltd. All rights reserved.
// </copyright>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DataTransfer.Models;
using Newtonsoft.Json;
using Orleans;
using Yuyi.Jinyinmao.Domain;
using Yuyi.Jinyinmao.Domain.Dtos;

namespace DataTransfer
{
    /// <summary>
    ///     MemoryWork.
    /// </summary>
    public class MemoryWork
    {
        [SuppressMessage("ReSharper", "NotAccessedField.Local")]
        private static readonly Guid JBYProductId;

        private static readonly int ProductExecuteDataCount;
        private static readonly string StrDefaultJBYProductId = "5e35201f315e41d4b11f014d6c01feb8";
        private static readonly int UserExecuteDataCount;

        static MemoryWork()
        {
            string strJBYProductId = ConfigurationManager.AppSettings.Get("StrJBYProductId");
            strJBYProductId = string.IsNullOrEmpty(strJBYProductId) ? StrDefaultJBYProductId : strJBYProductId;
            JBYProductId = new Guid(strJBYProductId);
            if (GrainClient.IsInitialized)
            {
                return;
            }
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            GrainClient.Initialize(Path.Combine(baseDir, "ReleaseConfiguration.xml"));
            GrainClient.SetResponseTimeout(TimeSpan.FromMinutes(2));

            ProductExecuteDataCount = 2000;
            UserExecuteDataCount = 4000;
            int.TryParse(ConfigurationManager.AppSettings.Get("ProductExecuteDataCount"), out ProductExecuteDataCount);
            int.TryParse(ConfigurationManager.AppSettings.Get("UserExecuteDataCount"), out UserExecuteDataCount);
        }

        /// <summary>
        ///     Runs this instance.
        /// </summary>
        /// <returns>Task.</returns>
        public static async Task Run()
        {
            await ProductTaskAsync();
            await UserTaskAsync();
        }

        #region 创建多个数据迁移任务

        private static async Task ProductTaskAsync()
        {
            double count = await GetProductCountAsync();
            List<Task> list = new List<Task>();
            for (int i = 0; i < Math.Ceiling(count / ProductExecuteDataCount); i++)
            {
                list.Add(ProductMigrationAsync(ProductExecuteDataCount, i * ProductExecuteDataCount, i));
            }
            await Task.WhenAll(list.ToArray());
        }

        private static async Task UserTaskAsync()
        {
            double count = await GetUserCountAsync();
            List<Task> list = new List<Task>();
            for (int i = 0; i < Math.Ceiling(count / UserExecuteDataCount); i++)
            {
                list.Add(UserMigrationAsync(UserExecuteDataCount, i * UserExecuteDataCount, i));
            }
            await Task.WhenAll(list.ToArray());
        }

        #endregion 创建多个数据迁移任务

        #region 分批次数据转移

        private static async Task ProductMigrationAsync(int takeCount, int skipCount, int threadId)
        {
            using (var context = new OldDBContext())
            {
                List<string> list = await context.JsonProduct.AsNoTracking().OrderBy(x => x.ProductId).Skip(skipCount).Take(takeCount).Select(x => x.Data).ToListAsync();
                var datas = list.Select(x => JsonConvert.DeserializeObject<RegularProductMigrationDto>(x)).ToList();
                for (int i = 0; i < datas.Count(); i++)
                {
                    var product = datas.ElementAt(i);
                    await RegularProductFactory.GetGrain(Guid.NewGuid()).MigrateAsync(product);
                }
            }
        }

        private static async Task UserMigrationAsync(int takeCount, int skipCount, int threadId)
        {
            using (var context = new OldDBContext())
            {
                List<string> list = await context.JsonUser.AsNoTracking().OrderBy(x => x.UserId).Skip(skipCount).Take(takeCount).Select(x => x.Data).ToListAsync();
                foreach (UserMigrationDto item in list.Select(x => JsonConvert.DeserializeObject<UserMigrationDto>(x)))
                {
                    await UserFactory.GetGrain(Guid.NewGuid()).MigrateAsync(item);
                }
            }
        }

        #endregion 分批次数据转移

        #region 获取Product和User数量

        private static async Task<double> GetProductCountAsync()
        {
            using (OldDBContext context = new OldDBContext())
            {
                double count = await context.JsonProduct.AsNoTracking().Select(x => x.ProductId).CountAsync();
                return count;
            }
        }

        private static async Task<double> GetUserCountAsync()
        {
            using (OldDBContext context = new OldDBContext())
            {
                double count = await context.JsonUser.AsNoTracking().Select(x => x.UserId).CountAsync();
                return count;
            }
        }

        #endregion 获取Product和User数量
    }
}