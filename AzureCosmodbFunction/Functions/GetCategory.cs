using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AzureCosmodbFunction.Contracts;

namespace AzureCosmodbFunction.Functions
{
    public  class GetCategory
    {
        private readonly ILogger<GetCategory> _logger;

        private readonly ICategoryRepository _categoryRepository;
        private string id;

        public GetCategory(ICategoryRepository categoryRepository, ILogger<GetCategory> logger)
        {
            _logger = logger;
            _categoryRepository = categoryRepository;
        }

        [FunctionName("GetCategory")]
        public  async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "getCategoryById/id")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request for get Category");
            IActionResult result;
            try
            {
                var category = await _categoryRepository.GetCategoryById(id);
                if(category == null)
                {
                    result = new StatusCodeResult(StatusCodes.Status404NotFound);
                    _logger.LogInformation($"Category with id {id} does not exist");
                }

                result = new OkObjectResult(category);
            }
            catch(Exception ex)
            {
                _logger.LogError($"Internal server error. Exception thrown: {ex.Message}");
                result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return result;
        }
    }
}
