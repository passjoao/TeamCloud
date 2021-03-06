﻿/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TeamCloud.Model.Common;
using TeamCloud.Serialization;

namespace TeamCloud.Model.Data
{
    [JsonObject(NamingStrategyType = typeof(TeamCloudNamingStrategy))]
    public sealed class ProjectLink : IProjectLink, IEquatable<ProjectLink>, IValidatable
    {
        [JsonProperty(Required = Required.Always)]
        public string Id { get; set; }
            = Guid.NewGuid().ToString();

        [JsonProperty("href", Required = Required.Always)]
        public string HRef { get; set; }

        public string Title { get; set; }

        [JsonProperty(Required = Required.Always)]
        public ProjectLinkType Type { get; set; }
            = ProjectLinkType.Link;

        public bool Equals(ProjectLink other)
            => Id.Equals(other?.Id, StringComparison.Ordinal);

        public override bool Equals(object obj)
            => base.Equals(obj) || Equals(obj as ProjectLink);

        public override int GetHashCode()
            => HashCode.Combine(Id, HRef);
    }
}
