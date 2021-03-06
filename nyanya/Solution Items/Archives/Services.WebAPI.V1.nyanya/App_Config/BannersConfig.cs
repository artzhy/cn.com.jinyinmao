﻿// FileInformation: nyanya/Services.WebAPI.V1.nyanya/BannersConfig.cs
// CreatedTime: 2014/04/20   9:44 PM
// LastUpdatedTime: 2014/04/21   9:25 PM

using System.Collections.Generic;
using Infrastructure.Cache.Couchbase;
using Newtonsoft.Json.Linq;

namespace Services.WebAPI.V1.nyanya.App_Config
{
    /// <summary>
    ///     BannersConfig
    /// </summary>
    public class BannersConfig : Config
    {
        #region Public Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BannersConfig" /> class.
        /// </summary>
        public BannersConfig()
        {
            JObject jObject = MeowConfigurationsCache.GetBanners();
            this.jsonSettings = jObject;
            this.ConfigSettings();
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        ///     Gets the banners.
        /// </summary>
        /// <value>
        ///     The banners.
        /// </value>
        public IEnumerable<Banner> Banners { get; private set; }

        /// <summary>
        ///     Gets the root.
        /// </summary>
        /// <value>
        ///     The root.
        /// </value>
        public string Root { get; private set; }

        /// <summary>
        ///     Gets the size.
        /// </summary>
        /// <value>
        ///     The size.
        /// </value>
        public Dictionary<string, string> Size { get; private set; }

        #endregion Public Properties

        #region Private Methods

        /// <summary>
        ///     Configurate the settings.
        /// </summary>
        private void ConfigSettings()
        {
            this.Banners = this.jsonSettings.GetValue("Banners").ToObject<IEnumerable<Banner>>();
            this.Root = this.jsonSettings.GetValue("Root").ToObject<string>();
            this.Size = this.jsonSettings.GetValue("Size").ToObject<Dictionary<string, string>>();
        }

        #endregion Private Methods

        #region Nested type: Banner

        /// <summary>
        ///     Banner
        /// </summary>
        public class Banner
        {
            #region Public Properties

            /// <summary>
            ///     Src
            /// </summary>
            /// <value>
            ///     The source.
            /// </value>
            public string Src { get; set; }

            /// <summary>
            ///     Url
            /// </summary>
            /// <value>
            ///     The URL.
            /// </value>
            public string Url { get; set; }

            #endregion Public Properties
        }

        #endregion Nested type: Banner
    }
}