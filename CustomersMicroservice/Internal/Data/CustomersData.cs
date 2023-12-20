using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using CustomersMicroservice.DataAccess;
using CustomersMicroservice.Models;
using System.Web.Http;

namespace CustomersMicroservice.Internal.Data
{
    public class CustomersData
    {
        private readonly ICustomersRepository _customersRepository;

        public CustomersData(ICustomersRepository customersRepository)
        {
            _customersRepository = customersRepository;
        }


        [FunctionName("CreateOrQuery")]
        public async Task<IActionResult> CreateOrGetAll(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "internal/data/customers")] HttpRequest req,
            ILogger log)
        {
            if (req.Method == HttpMethods.Post)
            {
                try
                {
                    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                    Customer customer = JsonConvert.DeserializeObject<Customer>(requestBody);
                    _customersRepository.Add(customer);
                    return new OkObjectResult(customer);
                }
                catch(Exception ex)
                {
                    return new BadRequestErrorMessageResult("Invalid request body -- " + ex.Message);
                }

            }
            else if (req.Method == HttpMethods.Get)
            {
                if (req.Query.Count == 0)
                {
                    var customers = _customersRepository.GetAll();
                    return new OkObjectResult(customers);
                }
                else
                {
                    var queryParams = req.GetQueryParameterDictionary();
                    if(queryParams.ContainsKey("minDate") || queryParams.ContainsKey("maxDate"))
                    {
                        var minParam = queryParams["minDate"];
                        var maxParam = queryParams["maxDate"];
                        
                        if(!DateOnly.TryParse(minParam, out DateOnly minDate) ||
                            !DateOnly.TryParse(maxParam, out DateOnly maxDate))
                        {
                            return new BadRequestErrorMessageResult("Invalid birthdate query parameters");
                        }    

                        var customers = _customersRepository.GetByBirthdate(minDate: minDate, maxDate: maxDate);
                        return new OkObjectResult(customers);
                    }
                }
                
            }


            return new BadRequestResult();
        }

        [FunctionName("UpdateDeleteOrGetSpecific")]
        public async Task<IActionResult> UpdateDeleteOrGetSpecific(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "put", "delete", Route = "internal/data/customers/{id:guid}")] HttpRequest req,
            Guid id,
            ILogger log)
        {
            if (req.Method == HttpMethods.Delete)
            {
                _customersRepository.Delete(id);
                return new OkResult();

            }
            else if (req.Method == HttpMethods.Get)
            {
                var customers = _customersRepository.GetById(id);
                return new OkObjectResult(customers);
            }
            else if (req.Method == HttpMethods.Put)
            {
                try
                {
                    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                    Customer customer = JsonConvert.DeserializeObject<Customer>(requestBody);
                    _customersRepository.Update(id, customer);
                    return new OkObjectResult(customer);
                }
                catch
                {
                    throw new ArgumentException("Invalid request body");
                }

            }


            return new BadRequestResult();
        }
    }
}
