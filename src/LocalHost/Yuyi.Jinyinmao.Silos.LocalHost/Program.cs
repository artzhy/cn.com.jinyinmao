// ***********************************************************************
// Project          : io.yuyi.jinyinmao.server
// Author           : Siqi Lu
// Created          : 2015-04-21  12:13 AM
//
// Last Modified By : Siqi Lu
// Last Modified On : 2015-05-27  7:17 PM
// ***********************************************************************
// <copyright file="Program.cs" company="Shanghai Yuyi Mdt InfoTech Ltd.">
//     Copyright ©  2012-2015 Shanghai Yuyi Mdt InfoTech Ltd. All rights reserved.
// </copyright>
// ***********************************************************************

using System;

namespace Yuyi.Jinyinmao.Silos.LocalHost
{
    internal class Program
    {
        private static OrleansHostWrapper hostWrapper;

        private static void InitSilo(string[] args)
        {
            try
            {
                hostWrapper = new OrleansHostWrapper();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            if (!hostWrapper.Run())
            {
                Console.Error.WriteLine("Failed to initialize Orleans silo");
            }
        }

        private static void Main(string[] args)
        {
            try
            {
                // The Orleans silo environment is initialized in its own app domain in order to more
                // closely emulate the distributed situation, when the client and the server cannot
                // pass data via shared memory.
                AppDomain hostDomain = AppDomain.CreateDomain("Yuyi.Jinyinmao.Domain.Service", null, new AppDomainSetup
                {
                    AppDomainInitializer = InitSilo,
                    AppDomainInitializerArguments = args
                });

                string command;

                do
                {
                    command = (Console.ReadLine() ?? "").ToUpperInvariant();
                } while (command != "SHUTDOWN");

                hostDomain.DoCallBack(ShutdownSilo);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void ShutdownSilo()
        {
            if (hostWrapper != null)
            {
                hostWrapper.Dispose();
                GC.SuppressFinalize(hostWrapper);
            }
        }
    }
}