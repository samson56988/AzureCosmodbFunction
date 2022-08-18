using AzureCosmodbFunction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureCosmodbFunction.Contracts
{
    public interface ICategoryRepository
    {
        Task<List<CategoryModel>> GetCategories();
        Task<CategoryModel> GetCategoryById(string id);
        Task CreateCategory(CategoryModel category);
        Task UpdateCategory(CategoryModel category, string Id, string name); 

        Task DeleteCategory(CategoryModel category);
        
    }
}
