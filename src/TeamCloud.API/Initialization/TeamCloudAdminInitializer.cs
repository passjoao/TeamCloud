﻿/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Flurl.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TeamCloud.API.Services;
using TeamCloud.Azure;
using TeamCloud.Data;
using TeamCloud.Model.Commands;
using TeamCloud.Model.Commands.Core;
using TeamCloud.Model.Data;

namespace TeamCloud.API.Initialization
{
    public class TeamCloudAdminInitializer : IHostInitializer
    {
        private readonly IAzureSessionService sessionService;
        private readonly IUserRepository userRepository;
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly Orchestrator orchestrator;
        private readonly ILoggerFactory loggerFactory;

        public TeamCloudAdminInitializer(IAzureSessionService sessionService, IUserRepository userRepository, IWebHostEnvironment hostingEnvironment, Orchestrator orchestrator, ILoggerFactory loggerFactory)
        {
            this.sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.hostingEnvironment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
            this.orchestrator = orchestrator ?? throw new ArgumentNullException(nameof(orchestrator));
            this.loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        private async Task<string> ResolveAzureCliUserId()
        {
            var token = await new AzureServiceTokenProvider("RunAs=Developer;DeveloperTool=AzureCLI")
                .GetAccessTokenAsync(sessionService.Environment.GetEndpointUrl(AzureEndpoint.ResourceManagerEndpoint))
                .ConfigureAwait(false);

            return new JwtSecurityTokenHandler()
                .ReadJwtToken(token).Payload
                .GetValueOrDefault("oid", StringComparison.OrdinalIgnoreCase) as string;
        }

        public async Task InitializeAsync()
        {
            var log = loggerFactory.CreateLogger(this.GetType());

            if (!hostingEnvironment.IsDevelopment())
            {
                log.LogInformation($"Hosting environment is '{hostingEnvironment.EnvironmentName}' - skip admin initialization");

                return;
            }

            try
            {
                var exists = await userRepository
                    .ListAdminsAsync()
                    .AnyAsync()
                    .ConfigureAwait(false);

                if (exists)
                {
                    log.LogInformation($"Environment is initialized with at least 1 admin user.");

                    return;
                }

                var user = new UserDocument
                {
                    Id = await ResolveAzureCliUserId().ConfigureAwait(false),
                    Role = TeamCloudUserRole.Admin,
                    UserType = UserType.User
                };

                var command = new OrchestratorTeamCloudUserCreateCommand(user, user);
                var commandResult = (ICommandResult)command.CreateResult();

                var commandSendDuration = TimeSpan.FromMinutes(5);
                var commandSendTimeout = DateTime.UtcNow.Add(commandSendDuration);

                while (DateTime.UtcNow < commandSendTimeout)
                {
                    try
                    {
                        commandResult = await orchestrator
                            .InvokeAsync(command)
                            .ConfigureAwait(false);

                        break;
                    }
                    catch (FlurlHttpException exc) when (exc.Call.HttpStatus == System.Net.HttpStatusCode.BadGateway)
                    {
                        log.LogWarning(exc, $"Failed to initialize environment with user '{user.Id}' - will retry in a second");

                        await Task
                            .Delay(1000)
                            .ConfigureAwait(false);
                    }
                }

                var commandResultDuration = TimeSpan.FromMinutes(5);
                var commandResultTimeout = DateTime.UtcNow.Add(commandResultDuration);

                while (DateTime.UtcNow < commandResultTimeout && commandResult.RuntimeStatus.IsActive())
                {
                    await Task
                        .Delay(1000)
                        .ConfigureAwait(false);

                    commandResult = await orchestrator
                        .QueryAsync(command)
                        .ConfigureAwait(false);
                }

                if (commandResult.RuntimeStatus == CommandRuntimeStatus.Completed)
                {
                    log.LogInformation($"Initialized environment with user '{user.Id}' as admin");
                }
                else
                {
                    log.LogWarning($"Failed to initialize environment with user '{user.Id}': {commandResult.Errors.FirstOrDefault()?.Message ?? commandResult.RuntimeStatus.ToString()}");
                }
            }
            catch (Exception exc)
            {
                log.LogError(exc, $"Failed to process initializer {this.GetType().Name}: {exc.Message}");
            }
        }
    }
}
