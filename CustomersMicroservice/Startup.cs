using CustomersMicroservice.DataAccess;
using CustomersMicroservice.DataAccess.Database;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;

[assembly: FunctionsStartup(typeof(CustomersMicroservice.Startup))]

namespace CustomersMicroservice
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<CustomerDatabase>();
            builder.Services.AddScoped<ICustomersRepository, CustomersRepository>();
            builder.Services.AddHttpClient();
        }

    }
 
}
