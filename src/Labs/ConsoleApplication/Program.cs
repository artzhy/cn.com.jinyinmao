// ***********************************************************************
// Project          : io.yuyi.jinyinmao.server
// File             : Program.cs
// Created          : 2015-08-13  15:17
//
// Last Modified By : Siqi Lu
// Last Modified On : 2015-08-17  1:54
// ***********************************************************************
// <copyright file="Program.cs" company="Shanghai Yuyi Mdt InfoTech Ltd.">
//     Copyright ©  2012-2015 Shanghai Yuyi Mdt InfoTech Ltd. All rights reserved.
// </copyright>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Moe.Lib;
using Yuyi.Jinyinmao.Packages.Helper;
using Yuyi.Jinyinmao.Service;

namespace ConsoleApplication
{
    public class Apple
    {
        public string Color { get; set; }
        public Guid Id { get; set; }
        public List<string> Properties { get; set; }
        public int Size { get; set; }
    }

    internal class Program
    {
        private static async Task DoSomething()
        {
            List<string> transactionIdentifiers = new List<string>
            {
                "d926dff2ca0d46d0826df0d0de00d98c",
                "257bf36b602d4b0bab1ceb6ced06d5c3",
                "572548d075754d678e6b51cab2e0c7af"
            };

            Console.WriteLine(transactionIdentifiers.Count);

            YilianPaymentGatewayService service = new YilianPaymentGatewayService();

            List<string> s = new List<string>();
            List<string> f = new List<string>();

            foreach (string identifier in transactionIdentifiers)
            {
                YilianRequestResult result;
                do
                {
                    result = await service.QueryRequestAsync(identifier, true);
                } while (result == null);

                if (result.Message.Contains("成功"))
                {
                    s.Add(identifier);
                    Console.WriteLine("!!!!!!!!" + identifier + result.Message);
                }
                else
                {
                    f.Add(identifier);
                    Console.WriteLine("????????" + identifier + result.Message);
                }
            }

            Console.WriteLine(s.Join("|"));
            Console.WriteLine();
            Console.WriteLine(f.Join("|"));
        }

        private static DateTime GetLastInvestingConfirmTime(DateTime date)
        {
            DailyConfig confirmConfig = DailyConfigHelper.GetLastWorkDayConfig(date, 1);
            return confirmConfig.Date.Date.AddDays(1).AddMilliseconds(-1);
        }

        private static void Main(string[] args)
        {
            Regex r = new Regex("^https://[^\\s/$.?#].[^\\s]*$");
            var result = r.IsMatch("https://files2.jinyinmao.com.cn/abcdiow?t=desd&y=122&_=121");
            Console.WriteLine(result);
        }
    }
}