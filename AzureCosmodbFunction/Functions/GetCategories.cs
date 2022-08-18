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
    public  class GetCategories
    {
        private readonly ILogger<GetCategories> _logger;

        private readonly ICategoryRepository _categoryRepository;
        private string id;

        public GetCategories(ICategoryRepository categoryRepository, ILogger<GetCategories> logger)
        {
            _logger = logger;
            _categoryRepository = categoryRepository;
        }

        [FunctionName("GetCategories")]
        public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "getCategories")] HttpRequest req)
        {
            IActionResult result;
            _logger.LogInformation("C# HTTP trigger function processed a request for get Category");

            try
            {
                var categories = await _categoryRepository.GetCategories();
                if(categories == null)
                {
                    result = new StatusCodeResult(StatusCodes.Status404NotFound);
                    _logger.LogInformation($"Categories not found");

                }

                result = new OkObjectResult(categories);
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
