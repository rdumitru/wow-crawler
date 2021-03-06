﻿using Microsoft.Azure;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WoW.Crawler.Model.Enum;
using WoW.Crawler.Service.Converters;

namespace WoW.Crawler.Service.Client
{
    public abstract class BattleNetClientBase
    {
        #region Private Fields

        private readonly string _locale;
        private readonly IEnumerable<string> _apiKeys;
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        #endregion Private Fields

        #region Protected Fields

        protected readonly HttpClient _clientEU;
        protected readonly HttpClient _clientUS;

        #endregion Protected Fields

        #region Public Constants

        public const string BattleNetApiBaseUrlEU = @"https://eu.api.battle.net";
        public const string BattleNetApiBaseUrlUS = @"https://us.api.battle.net";
        public const string Locale = @"en_US";

        #endregion Public Constants

        #region Constructors

        public BattleNetClientBase()
        {
            // Json serializer settings.
            this._jsonSerializerSettings = new JsonSerializerSettings();
            this._jsonSerializerSettings.Converters.Add(new StringEnumConverter());
            this._jsonSerializerSettings.Converters.Add(new MillisecondsToDateTimeConverter());
            this._jsonSerializerSettings.NullValueHandling = NullValueHandling.Ignore;

            // TODO: perhaps make this a parameter?
            // Get values from the config.
            this._apiKeys = CloudConfigurationManager.GetSetting("BattleNetApi.Keys")
                .Split('|').Where(key => !String.IsNullOrWhiteSpace(key)).ToList();

            // Create the HTTP client with the given base URL.
            this._clientEU = new HttpClient { BaseAddress = new Uri(BattleNetApiBaseUrlEU) };
            this._clientUS = new HttpClient { BaseAddress = new Uri(BattleNetApiBaseUrlUS) };
        }

        #endregion Constructors

        #region Protected Helpers

        protected HttpClient GetClient(Region region)
        {
            switch (region)
            {
                case Region.EU:
                    return this._clientEU;

                case Region.US:
                    return this._clientUS;

                default:
                    throw new ArgumentException();
            }
        }

        protected string BuildQueryStr(string apiKey, NameValueCollection keyValuePairs = null)
        {
            // Avoid a null reference.
            keyValuePairs = keyValuePairs ?? new NameValueCollection();

            // Locale and API key.
            var baseQueryStr = String.Format("?locale={0}&apikey={1}", Uri.EscapeDataString(Locale), Uri.EscapeDataString(apiKey));

            // Optional query string fields.
            var nvc = HttpUtility.ParseQueryString(String.Empty);
            foreach (string key in keyValuePairs)
            {
                nvc.Add(key, keyValuePairs[key]);
            }

            // Base QS is already URL encoded. NVC to String will produce URL encoded output.
            var fullQueryStr = baseQueryStr
                + (nvc.Count > 0 ? '&' + nvc.ToString() : String.Empty);

            return fullQueryStr;
        }

        protected async Task<HttpResponseMessage> GetAsync(Region region, string relativeUrl, NameValueCollection queryStrNvc = null)
        {
            HttpResponseMessage lastResponse = null;
            foreach (var apiKey in this._apiKeys)
            {
                // Build URL.
                var url = relativeUrl + this.BuildQueryStr(apiKey, queryStrNvc);

                // Make GET call.
                var response = await this.GetClient(region).GetAsync(url);
                if (response.IsSuccessStatusCode) return response;
                lastResponse = response;
            }

            lastResponse.EnsureSuccessStatusCode();
            return lastResponse; // WARN: Should never reach here.
        }

        protected Task<StringContent> SerializeContentAsync<T>(T obj)
        {
            return Task.Run(() =>
            {
                var serializedObject = JsonConvert.SerializeObject(obj, Formatting.None, this._jsonSerializerSettings);
                var stringContent = new StringContent(serializedObject, Encoding.UTF8, "application/json");

                return stringContent;
            });
        }

        protected async Task<T> DeserializeContentAsync<T>(HttpContent content)
        {
            var stringContent = await content.ReadAsStringAsync();

            return await Task.Run(() =>
            {
                var obj = JsonConvert.DeserializeObject<T>(stringContent, this._jsonSerializerSettings);
                return obj;
            });
        }

        #endregion Protected Helpers
    }
}