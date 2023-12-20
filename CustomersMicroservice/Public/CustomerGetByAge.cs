using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using CustomersMicroservice.Models;
using System.Net.Http;
using System.Web.Http;
using System.Collections.Generic;

namespace CustomersMicroservice.Public
{
    public class CustomerGetByAge
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public CustomerGetByAge(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [FunctionName("CustomerGetByAge")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "customers/{age:int}")] HttpRequest req,
            int age,
            ILogger log)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var datesRequestMessage = new HttpRequestMessage(HttpMethod.Get, Environment.GetEnvironmentVariable("BaseUrl") + "/internal/birthdaterange/" + age);

            var dateResponse = await httpClient.SendAsync(datesRequestMessage);

            DateRange range;
            try
            {
                var response = await dateResponse.Content.ReadAsStringAsync();
                range = JsonConvert.DeserializeObject<DateRange>(response);
            }
            catch (Exception ex)
            {
                var badResult = new ObjectResult("Error calculating birthdate range implied by specified age.  " + ex.Message)
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
                return badResult;
            }

            var requestUrl = $"{Environment.GetEnvironmentVariable("BaseUrl")}/internal/data/customers?minDate={range.MinDate:yyyy-MM-dd}&maxDate={range.MaxDate:yyyy-MM-dd}";
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUrl);

            var res = await httpClient.SendAsync(requestMessage);

            List<Customer> customers;
            try
            {
                var response = await res.Content.ReadAsStringAsync();
                customers = JsonConvert.DeserializeObject<List<Customer>>(response);
            }
            catch (Exception ex)
            {
                var badResult = new ObjectResult("Error parsing response  " + ex.Message)
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
                return badResult;
            }

               return new OkObjectResult(customers);
        }
    }
}
