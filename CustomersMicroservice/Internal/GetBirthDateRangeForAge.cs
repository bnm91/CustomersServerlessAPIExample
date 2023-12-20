using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using CustomersMicroservice.Business;

namespace CustomersMicroservice.Internal
{
    public static class GetBirthDateRangeForAge
    {

        [FunctionName("GetBirthDateRangeForAge")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "internal/birthdaterange/{age:int}")] HttpRequest req,
            int age,
            ILogger log)
        {
            var birthDateRange = BirthDateCalculator.GetBirthDateRangeForAge(age);
            return new OkObjectResult(birthDateRange);
        }
    }
}
