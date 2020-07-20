using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pulsar.Customers.Api.Data.Services;
using Pulsar.Customers.Api.Infrastructure.HealthServices;
using Pulsar.Customers.Api.Models.Customers;

namespace Pulsar.Customers.Api.Controllers
{

    [Route("api/customers")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ILogger<CustomersController> _logger;
        private readonly IPersistentStorageService<CustomerViewModel, Customer> _persistentStorageService;
        private readonly HealthService _healthService;

        public CustomersController(ILogger<CustomersController> logger,
            IPersistentStorageService<CustomerViewModel, Customer> persistentStorageService,
            HealthService healthService)
        {
            _logger = logger;
            _persistentStorageService = persistentStorageService;
            _healthService = healthService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCustomers()
        {
            if (!_healthService.IsStateHealthy) return StatusCode(StatusCodes.Status500InternalServerError);

            try
            {
                var resultFromStorage = await _persistentStorageService.GetAllAsync();
                return Ok(resultFromStorage);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }

        [HttpGet("search/{name}")]
        public async Task<IActionResult> GetCustomerByName(string name)
        {
            if (!_healthService.IsStateHealthy) return StatusCode(StatusCodes.Status500InternalServerError);

            try
            {
                var resultFromStorage =
                    await _persistentStorageService.GetByExpressionAsync(x => x.Name.Contains(name));
                return Ok(resultFromStorage);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("{id}", Name = "GetCustomerId")]
        public async Task<IActionResult> GetCustomerById(string id)
        {
            if (!_healthService.IsStateHealthy) return StatusCode(StatusCodes.Status500InternalServerError);
            if (!Guid.TryParse(id, out var customerId)) return BadRequest();

            try
            {
                var resultFromStorage = await _persistentStorageService.GetByExpressionAsync(x => x.Id == customerId);
                if (resultFromStorage.FirstOrDefault() == null) return NotFound();
                return Ok(resultFromStorage.FirstOrDefault());
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] CustomerViewModel customer)
        {

            if (!_healthService.IsStateHealthy) return StatusCode(StatusCodes.Status500InternalServerError);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var newCustomer = await _persistentStorageService.CreateAsync(customer);
                return CreatedAtRoute("GetCustomerId", new CustomerViewModel {Id = newCustomer},
                    newCustomer.ToString());
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("{id}", Name = "UpdateCustomer")]
        public async Task<IActionResult> UpdateCustomer(string id, [FromBody] CustomerViewModel customer)
        {

            if (!_healthService.IsStateHealthy) return StatusCode(StatusCodes.Status500InternalServerError);
            if (!Guid.TryParse(id, out var customerId)) return BadRequest();

            if (customerId != customer.Id) return BadRequest();


            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _persistentStorageService.UpdateAsync(customer);
                return Ok();

            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
            
    



}
}
