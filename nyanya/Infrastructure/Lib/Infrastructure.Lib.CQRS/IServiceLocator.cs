﻿// FileInformation: nyanya/Infrastructure.Lib.CQRS/IServiceLocator.cs
// CreatedTime: 2014/06/04   12:53 PM
// LastUpdatedTime: 2014/06/04   12:54 PM

using System;

namespace Infrastructure.Lib.CQRS
{
    public interface IServiceLocator : IServiceProvider
    {
        /// <summary>
        ///     Gets the service object of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the service object.</typeparam>
        /// <returns>The instance of the service object.</returns>
        T GetService<T>() where T : class;

        /// <summary>
        ///     Gets the service object of the specified type, with overrided
        ///     arguments provided.
        /// </summary>
        /// <typeparam name="T">The type of the service object.</typeparam>
        /// <param name="overridedArguments">The overrided arguments to be used when getting the service.</param>
        /// <returns>The instance of the service object.</returns>
        T GetService<T>(object overridedArguments) where T : class;

        /// <summary>
        ///     Gets the service object of the specified type, with overrided
        ///     arguments provided.
        /// </summary>
        /// <param name="serviceType">The type of the service to get.</param>
        /// <param name="overridedArguments">The overrided arguments to be used when getting the service.</param>
        /// <returns>The instance of the service object.</returns>
        object GetService(Type serviceType, object overridedArguments);

        /// <summary>
        ///     Returns a <see cref="Boolean" /> value which indicates whether the given type
        ///     has been registered to the service locator.
        /// </summary>
        /// <typeparam name="T">The type to check.</typeparam>
        /// <returns>True if the type has been registered, otherwise, false.</returns>
        bool Registered<T>();

        /// <summary>
        ///     Returns a <see cref="Boolean" /> value which indicates whether the given type
        ///     has been registered to the service locator.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>True if the type has been registered, otherwise, false.</returns>
        bool Registered(Type type);

        /// <summary>
        ///     Resolves all the objects from the specified type.
        /// </summary>
        /// <param name="serviceType">The type of the objects to be resolved.</param>
        /// <returns>A <see cref="System.Array" /> object which contains all the objects resolved.</returns>
        Array ResolveAll(Type serviceType);

        /// <summary>
        ///     Resolves all the objects from the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the objects to be resolved.</typeparam>
        /// <returns>A <see cref="System.Array" /> object which contains all the objects resolved.</returns>
        T[] ResolveAll<T>() where T : class;
    }
}