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
using System.Text;
using AzureCosmodbFunction.Models;

namespace AzureCosmodbFunction.Functions
{
    public class CreateCategory
    {


        private readonly ILogger<CreateCategory> _logger;

        private readonly ICategoryRepository _categoryRepository;

        public CreateCategory(ICategoryRepository categoryRepository, ILogger<CreateCategory> logger)
        {
            _logger = logger;
            _categoryRepository = categoryRepository;
        }


        [FunctionName("AddCategory")]
        public  async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "createCategory")] HttpRequest req,ILogger log)
        {

            IActionResult result = null;
            try
            {
                using var read = new StreamReader(req.Body, Encoding.UTF8);
                var incomingReq = await read.ReadToEndAsync();

                if (!string.IsNullOrEmpty(incomingReq))
                {
                    var categoryReq = JsonConvert.DeserializeObject<Models.CategoryModel>(incomingReq);


                    var categoryModel = new CategoryModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        Description = categoryReq.Description,
                        IsActive = categoryReq.IsActive,
                        Name = categoryReq.Name
                    };

                    await _categoryRepository.CreateCategory(categoryModel);
                    result = new StatusCodeResult(StatusCodes.Status201Created);

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
