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
using AzureCosmodbFunction.Models;
using System.Text;

namespace AzureCosmodbFunction.Functions
{
    public class DeleteCategory
    {
        private readonly ILogger<DeleteCategory> _logger;

        private readonly ICategoryRepository _categoryRepository;
        private string id;

        public DeleteCategory(ICategoryRepository categoryRepository, ILogger<DeleteCategory> logger)
        {
            _logger = logger;
            _categoryRepository = categoryRepository;
        }

        [FunctionName("DeleteCategory")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "deleteCategory/{id}")] HttpRequest req)
        {
            IActionResult result;
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                using var read = new StreamReader(req.Body, Encoding.UTF8);
                var incomingReq = await read.ReadToEndAsync();

                if(!string.IsNullOrEmpty(incomingReq))
                {
                    var categoryReq =  JsonConvert.DeserializeObject<CategoryModel>(incomingReq);
                    await _categoryRepository.DeleteCategory(categoryReq);
                    result = new StatusCodeResult(StatusCodes.Status204NoContent);
                }
                else
                {
                    result = new StatusCodeResult(StatusCodes.Status400BadRequest);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError($"internal Server Error, Exception: {ex.Message}");
                result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return result;

            
        }
    }
}
