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
    public class UpdateCategory
    {
        private readonly ILogger<UpdateCategory> _logger;

        private readonly ICategoryRepository _categoryRepository;
        private string id;

        public UpdateCategory(ICategoryRepository categoryRepository, ILogger<UpdateCategory> logger)
        {
            _logger = logger;
            _categoryRepository = categoryRepository;
        }

        [FunctionName("UpdateCategory")]
        public  async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "updateCategory/id")] HttpRequest req)
            
        {
            IActionResult result;
            _logger.LogInformation("C# HTTP trigger function processed a request for update category.");

            try
            {
                var category = _categoryRepository.GetCategoryById(id);

                if(category == null)
                {
                    result = new StatusCodeResult(StatusCodes.Status404NotFound);
                    _logger.LogInformation($"Category with Id {id} does not exist");
                }
                else
                {
                    using var r = new StreamReader(req.Body, Encoding.UTF8);
                    var IncomingRequest = await r.ReadToEndAsync();

                    if(!string.IsNullOrEmpty(IncomingRequest))
                    {
                        var CategorReq = JsonConvert.DeserializeObject<CategoryModel>(IncomingRequest);

                        var CategoryModel = new CategoryModel
                        {
                            Id = id,
                            Description = CategorReq.Description,
                            IsActive = CategorReq.IsActive,
                            Name =  CategorReq.Name
                        };

                        await _categoryRepository.UpdateCategory(CategoryModel,id, CategoryModel.Name);
                        result = new StatusCodeResult(StatusCodes.Status201Created);
                    }
                    else
                    {
                        result = new StatusCodeResult(StatusCodes.Status400BadRequest);
                    }
                }
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
