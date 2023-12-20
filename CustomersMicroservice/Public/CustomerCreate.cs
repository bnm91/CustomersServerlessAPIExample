using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Json;
using CustomersMicroservice.Models;
using System.Web.Http;
using System.Text;

namespace CustomersMicroservice.Public
{
    public class CustomerCreate
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public CustomerCreate(IHttpClientFactory httpClientFactory) 
        { 
            _httpClientFactory = httpClientFactory;
        }

        [FunctionName("CustomerCreate")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "customers")] HttpRequest req,
            ILogger log)
        {
            Customer customer;
            string requestBody;
            try
            {
                requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            }
            catch
            {
                return new BadRequestErrorMessageResult("Customer object must be properly constructed");
            }

            var httpClient = _httpClientFactory.CreateClient();
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, Environment.GetEnvironmentVariable("BaseUrl") + "/internal/data/customers")
            {
                Content = new StringContent(requestBody, Encoding.UTF8, "application/json")
            };


            var res = await httpClient.SendAsync(requestMessage);

            try
            {
                requestBody = await res.Content.ReadAsStringAsync();
                customer = JsonConvert.DeserializeObject<Customer>(requestBody);
            }
            catch(Exception ex)
            {
                return new ExceptionResult(ex, true);
            }

            return new OkObjectResult(customer);
        }
    }
}
