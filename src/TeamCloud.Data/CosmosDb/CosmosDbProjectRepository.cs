﻿/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using TeamCloud.Data.CosmosDb.Core;
using TeamCloud.Model.Data;
using TeamCloud.Model.Validation;

namespace TeamCloud.Data.CosmosDb
{
    public class CosmosDbProjectRepository : CosmosDbRepository<ProjectDocument>, IProjectRepository
    {
        private readonly IUserRepository userRepository;
        private readonly IProjectLinkRepository projectLinkRepository;

        public CosmosDbProjectRepository(ICosmosDbOptions cosmosOptions, IUserRepository userRepository, IProjectLinkRepository projectLinkRepository)
            : base(cosmosOptions)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.projectLinkRepository = projectLinkRepository ?? throw new ArgumentNullException(nameof(projectLinkRepository));
        }

        public async Task<ProjectDocument> AddAsync(ProjectDocument project)
        {
            if (project is null)
                throw new ArgumentNullException(nameof(project));

            await project
                .ValidateAsync(throwOnValidationError: true)
                .ConfigureAwait(false);

            var container = await GetContainerAsync()
                .ConfigureAwait(false);

            try
            {
                var response = await container
                    .CreateItemAsync(project)
                    .ConfigureAwait(false);

                return await PopulateUsersAsync(response.Resource)
                    .ConfigureAwait(false);
            }
            catch (CosmosException cosmosEx) when (cosmosEx.StatusCode == HttpStatusCode.Conflict)
            {
                throw; // Indicates a name conflict (already a project with name)
            }
        }

        public async Task<ProjectDocument> GetAsync(string nameOrId)
        {
            var container = await GetContainerAsync()
                .ConfigureAwait(false);

            ProjectDocument project = null;

            try
            {
                var response = await container
                    .ReadItemAsync<ProjectDocument>(nameOrId, new PartitionKey(Options.TenantName))
                    .ConfigureAwait(false);

                project = response.Resource;
            }
            catch (CosmosException cosmosEx) when (cosmosEx.StatusCode == HttpStatusCode.NotFound)
            {
                var query = new QueryDefinition($"SELECT * FROM c WHERE c.name = '{nameOrId}'");

                var queryIterator = container
                    .GetItemQueryIterator<ProjectDocument>(query, requestOptions: new QueryRequestOptions { PartitionKey = new PartitionKey(Options.TenantName) });

                if (queryIterator.HasMoreResults)
                {
                    var queryResults = await queryIterator
                        .ReadNextAsync()
                        .ConfigureAwait(false);

                    project = queryResults.FirstOrDefault();
                }
            }

            return await PopulateUsersAsync(project)
                .ConfigureAwait(false);
        }

        public async Task<bool> NameExistsAsync(string name)
        {
            var project = await GetAsync(name)
                .ConfigureAwait(false);

            return project != null;
        }

        public async Task<ProjectDocument> SetAsync(ProjectDocument project)
        {
            if (project is null)
                throw new ArgumentNullException(nameof(project));

            await project
                .ValidateAsync(throwOnValidationError: true)
                .ConfigureAwait(false);

            var container = await GetContainerAsync()
                .ConfigureAwait(false);

            var response = await container
                .UpsertItemAsync(project, new PartitionKey(Options.TenantName))
                .ConfigureAwait(false);

            return await PopulateUsersAsync(response.Resource)
                .ConfigureAwait(false);
        }

        public async IAsyncEnumerable<ProjectDocument> ListAsync()
        {
            var container = await GetContainerAsync()
                .ConfigureAwait(false);

            var query = new QueryDefinition($"SELECT * FROM p");

            var queryIterator = container
                .GetItemQueryIterator<ProjectDocument>(query, requestOptions: new QueryRequestOptions { PartitionKey = new PartitionKey(Options.TenantName) });

            while (queryIterator.HasMoreResults)
            {
                var queryResponse = await queryIterator
                    .ReadNextAsync()
                    .ConfigureAwait(false);

                foreach (var project in queryResponse)
                    yield return await PopulateUsersAsync(project)
                        .ConfigureAwait(false);
            }
        }

        public async IAsyncEnumerable<ProjectDocument> ListAsync(IEnumerable<string> nameOrIds)
        {
            var container = await GetContainerAsync()
                .ConfigureAwait(false);

            var search = "'" + string.Join("', '", nameOrIds) + "'";
            var query = new QueryDefinition($"SELECT * FROM p WHERE p.id IN ({search}) OR p.name in ({search})");

            var queryIterator = container
                .GetItemQueryIterator<ProjectDocument>(query, requestOptions: new QueryRequestOptions { PartitionKey = new PartitionKey(Options.TenantName) });

            while (queryIterator.HasMoreResults)
            {
                var queryResponse = await queryIterator
                    .ReadNextAsync()
                    .ConfigureAwait(false);

                foreach (var project in queryResponse)
                    yield return await PopulateUsersAsync(project)
                        .ConfigureAwait(false);
            }
        }


        public async IAsyncEnumerable<ProjectDocument> ListByProviderAsync(string providerId)
        {
            var container = await GetContainerAsync()
                .ConfigureAwait(false);

            var query = new QueryDefinition($"SELECT VALUE p FROM p WHERE EXISTS(SELECT VALUE t FROM t IN p.type.providers WHERE t.id = '{providerId}')");

            var queryIterator = container
                .GetItemQueryIterator<ProjectDocument>(query, requestOptions: new QueryRequestOptions { PartitionKey = new PartitionKey(Options.TenantName) });

            while (queryIterator.HasMoreResults)
            {
                var queryResponse = await queryIterator
                    .ReadNextAsync()
                    .ConfigureAwait(false);

                foreach (var project in queryResponse)
                    yield return await PopulateUsersAsync(project)
                        .ConfigureAwait(false);
            }
        }

        public async Task<ProjectDocument> RemoveAsync(ProjectDocument project)
        {
            if (project is null)
                throw new ArgumentNullException(nameof(project));

            var container = await GetContainerAsync()
                .ConfigureAwait(false);

            try
            {
                var response = await container
                    .DeleteItemAsync<ProjectDocument>(project.Id, new PartitionKey(Options.TenantName))
                    .ConfigureAwait(false);

                var tasks = new Task[]
                {
                    userRepository.RemoveProjectMembershipsAsync(project.Id),
                    projectLinkRepository.RemoveAsync(project.Id)
                };

                await Task
                    .WhenAll(tasks)
                    .ConfigureAwait(false);

                return response.Resource;
            }
            catch (CosmosException cosmosEx) when (cosmosEx.StatusCode == HttpStatusCode.NotFound)
            {
                return null; // already deleted
            }
        }

        private async Task<ProjectDocument> PopulateUsersAsync(ProjectDocument project)
        {
            if (project != null)
                project.Users = await userRepository
                    .ListAsync(project.Id)
                    .ToListAsync()
                    .ConfigureAwait(false);

            return project;
        }
    }
}
