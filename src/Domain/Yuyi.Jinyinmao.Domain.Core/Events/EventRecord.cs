// ***********************************************************************
// Project          : io.yuyi.jinyinmao.server
// File             : EventRecord.cs
// Created          : 2015-08-13  15:17
//
// Last Modified By : Siqi Lu
// Last Modified On : 2015-08-17  1:10
// ***********************************************************************
// <copyright file="EventRecord.cs" company="Shanghai Yuyi Mdt InfoTech Ltd.">
//     Copyright ©  2012-2015 Shanghai Yuyi Mdt InfoTech Ltd. All rights reserved.
// </copyright>
// ***********************************************************************

using System;
using Moe.Lib;

namespace Yuyi.Jinyinmao.Domain
{
    /// <summary>
    ///     EventEx.
    /// </summary>
    public static class EventEx
    {
        /// <summary>
        ///     To the record.
        /// </summary>
        /// <param name="event">The event.</param>
        /// <returns>EventRecord.</returns>
        public static EventRecord ToRecord(this IEvent @event) => new EventRecord
        {
            Event = @event.ToJson(),
            EventId = @event.EventId,
            EventName = @event.GetType().Name,
            SourceId = @event.SourceId
        };
    }

    /// <summary>
    ///     EventRecord.
    /// </summary>
    public class EventRecord
    {
        /// <summary>
        ///     Gets or sets the event.
        /// </summary>
        /// <value>The event.</value>
        public string Event { get; set; }

        /// <summary>
        ///     Gets or sets the event identifier.
        /// </summary>
        /// <value>The event identifier.</value>
        public Guid EventId { get; set; }

        /// <summary>
        ///     Gets or sets the name of the event.
        /// </summary>
        /// <value>The name of the event.</value>
        public string EventName { get; set; }

        /// <summary>
        ///     Gets or sets the source identifier.
        /// </summary>
        /// <value>The source identifier.</value>
        public string SourceId { get; set; }
    }
}