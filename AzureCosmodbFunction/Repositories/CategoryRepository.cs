using AzureCosmodbFunction.Contracts;
using AzureCosmodbFunction.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AzureCosmodbFunction.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {

        private CosmosClient cosmosClient;
        private Database database;
        private Container container;
        private string databaseId = string.Empty;
        private string containerId =  string.Empty;
        private IConfiguration _iconfig;

        private CategoryRepository(IConfiguration iconfig)
        {
            _iconfig = iconfig;
            string connectionString = iconfig["CosmosDbConnectionString"];
            databaseId = "categoriesfunctiondb";
            containerId = "categoryContainer";


            cosmosClient = new CosmosClient(connectionString, new CosmosClientOptions()
            {
                ConnectionMode = ConnectionMode.Gateway
            });


            CreateContainerAsync().Wait();
            CreateDatabaseAsync().Wait();
        }

        private async Task CreateDatabaseAsync()
        {
            this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
        }

        private async Task CreateContainerAsync()
        {
            this.container = await this.database.CreateContainerIfNotExistsAsync(containerId, "/name");
        }

        public async Task CreateCategory(CategoryModel category)
        {
            try
            {
                ItemResponse<CategoryModel> itemResponse = await container.ReadItemAsync<CategoryModel>(category.Id, new PartitionKey(category.Name));
            }
            catch(CosmosException ex) when (ex.StatusCode ==  HttpStatusCode.NotFound)
            {
                await container.CreateItemAsync(category, new PartitionKey(category.Name));
            }
        }

        public async Task<List<CategoryModel>> GetCategories()
        {
            var qry = "SELECT * FROM c";
            QueryDefinition queryDefinition =  new QueryDefinition(qry);
            FeedIterator<CategoryModel> queryIterator = container.GetItemQueryIterator<CategoryModel>(queryDefinition);

           List<CategoryModel> result = new List<CategoryModel>();

            while(queryIterator.HasMoreResults)
            {
                FeedResponse<CategoryModel> resultSet = await queryIterator.ReadNextAsync();
                foreach(CategoryModel categoryRes in resultSet)
                {
                    result.Add(categoryRes);
                }
            }

            return result;
        }

        public async Task<CategoryModel> GetCategoryById(string id)
        {
            try
            {
                var qry = string.Format("SElECT * FROM c where c.id = '{0}'", id);
                QueryDefinition queryDefinition =  new QueryDefinition(qry);
                FeedIterator<CategoryModel> queryIterator = container.GetItemQueryIterator<CategoryModel>(queryDefinition);

                CategoryModel result = new CategoryModel();

                while (queryIterator.HasMoreResults)
                {
                    FeedResponse<CategoryModel> resultSet =  await queryIterator.ReadNextAsync();
                    foreach(CategoryModel categoryModel in resultSet)
                    {
                        result.Id = categoryModel.Id;
                        result.Name = categoryModel.Name;
                        result.Description = categoryModel.Description;
                        result.IsActive = categoryModel.IsActive;
                    }
                }
                return result;
            }
            catch(CosmosException ex)
            {
                throw new System.Exception(String.Format("Error occurs: Error::{0}", ex.Message));
            }
        }

        public async Task UpdateCategory(CategoryModel category, string Id, string name)
        {
            ItemResponse<CategoryModel> response = await this.container.ReadItemAsync<CategoryModel>(Id, new PartitionKey(name));

            var result = response.Resource;

            result.Id = category.Id;
            result.Name = category.Name;
            result.Description = category.Description;
            result.IsActive = category.IsActive;

            await this.container.ReplaceItemAsync<CategoryModel>(result, result.Id, new PartitionKey(category.Name));
            
        }

        public async Task DeleteCategory(CategoryModel category)
        {
            var PartitionKeyValue = category.Name;
            var Id = category.Id;

            await this.container.DeleteItemAsync<CategoryModel>(Id, new PartitionKey(PartitionKeyValue));
        }

         
    }
}
