using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzureCosmodbFunction;
using AzureCosmodbFunction.Contracts;
using AzureCosmodbFunction.Repositories;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly:FunctionsStartup(typeof(Startup))]
namespace AzureCosmodbFunction
{
    public class Startup:FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder build)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            build.Services.AddSingleton<IConfiguration>(config);
            build.Services.AddTransient<ICategoryRepository, CategoryRepository>();
        }
    }
}
