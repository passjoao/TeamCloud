/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using TeamCloud.Model.Data;

namespace TeamCloud.Model.Commands
{
    public class OrchestratorCommandMessage : CommandMessage
    {
        public OrchestratorCommandMessage() : base()
        { }

        public OrchestratorCommandMessage(ICommand command) : base(command)
        { }

        public OrchestratorCommandMessage(ICommand command, TeamCloudInstance teamCloud) : base(command)
        {
            TeamCloud = teamCloud ?? throw new ArgumentNullException(nameof(teamCloud));
        }

        public TeamCloudInstance TeamCloud { get; set; }



    }

    public class OrchestratorCommandResultMessage
    {
        public ICommandResult CommandResult { get; set; }

        Dictionary<Provider, ICommandResult> ProviderCommandResults { get; set; } = new Dictionary<Provider, ICommandResult>();

        public OrchestratorCommandResultMessage() { }

        public OrchestratorCommandResultMessage(ICommandResult commandResult, Dictionary<Provider, ICommandResult> providerCommandResults)
        {
            CommandResult = commandResult;
            ProviderCommandResults = providerCommandResults;
        }

        [JsonIgnore]
        public Guid CommandId => CommandResult.CommandId;

        [JsonIgnore]
        public Dictionary<Provider, List<Exception>> ProviderExceptions => ProviderCommandResults
            .Where(pr => pr.Value.Exceptions.Any())
            .ToDictionary(pr => pr.Key, pr => pr.Value.Exceptions);

        [JsonIgnore]
        public List<Exception> Exceptions => CommandResult.Exceptions
            .Concat(ProviderCommandResults
                .Where(pr => pr.Value.Exceptions.Any())
                .SelectMany(pr => pr.Value.Exceptions)
            ).ToList();
    }
}