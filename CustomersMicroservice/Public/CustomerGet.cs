using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text;
using CustomersMicroservice.Models;
using Newtonsoft.Json;
using System.Web.Http;

namespace CustomersMicroservice.Public
{
    public class CustomerGet
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public CustomerGet(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [FunctionName("CustomerGet")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "customers/{id:guid}")] HttpRequest req,
            Guid id,
            ILogger log)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, Environment.GetEnvironmentVariable("BaseUrl") + "/internal/data/customers/" + id);

            var res = await httpClient.SendAsync(requestMessage);

            Customer customer;
            try
            {
                var response = await res.Content.ReadAsStringAsync();
                customer = JsonConvert.DeserializeObject<Customer>(response);
            }
            catch (Exception ex)
            {
                return new ExceptionResult(ex, true);
            }

            return new OkObjectResult(customer);
        }
    }
}
