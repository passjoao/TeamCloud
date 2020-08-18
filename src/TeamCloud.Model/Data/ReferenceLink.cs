﻿/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using TeamCloud.Model.Data.Serialization;

namespace TeamCloud.Model.Data
{
    [JsonConverter(typeof(ReferenceLinkConverter))]
    public sealed class ReferenceLink
    {
        private static readonly Regex TokenExpression
            = new Regex(@"\{(.+?)\}", RegexOptions.Compiled);

        private readonly Func<string> hrefFactory;

        public static string BaseUrl { get; set; }

        public ReferenceLink(Func<string> hrefFactory = null)
        {
            this.hrefFactory = hrefFactory;
        }

        private string href;

        [JsonProperty("href")]
        public string HRef
        {
            get => href ?? hrefFactory?.Invoke();
            set => href = value;
        }

        [JsonIgnore]
        public bool Materialized
            => !(href is null);

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool Templated
            => TokenExpression.IsMatch(HRef ?? string.Empty);

        public void Materialize()
            => href ??= hrefFactory?.Invoke();

        public override string ToString()
            => this.HRef;

        public string ToString(Func<string, string> tokenCallback)
        {
            if (tokenCallback is null)
                throw new ArgumentNullException(nameof(tokenCallback));

            return TokenExpression
                .Replace(HRef ?? string.Empty, new MatchEvaluator((Match match) => tokenCallback(match.Value.TrimStart('{').TrimEnd('}'))));
        }
    }
}