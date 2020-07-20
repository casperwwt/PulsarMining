using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pulsar.Inventory.Api.Data.Services;
using Pulsar.Inventory.Api.Infrastructure.HealthServices;
using Pulsar.Inventory.Api.Models.InventoryItems;

namespace Pulsar.Inventory.Api.Controllers
{
    [Route("api/inventory")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly ILogger<InventoryController> _logger;
        private readonly IPersistentStorageService<InventoryItemViewModel, InventoryItem> _persistentStorageService;
        private readonly HealthService _healthService;

        public InventoryController(ILogger<InventoryController> logger, IPersistentStorageService<InventoryItemViewModel, InventoryItem> persistentStorageService, HealthService healthService)
        {
            _logger = logger;
            _persistentStorageService = persistentStorageService;
            _healthService = healthService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllInventoryItemsAsync()
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

        [HttpGet("{id}", Name = "GetInventoryById")]
        public async Task<IActionResult> GetCustomerById(string id)
        {
            if (!_healthService.IsStateHealthy) return StatusCode(StatusCodes.Status500InternalServerError);
            if (!Guid.TryParse(id, out var result)) return BadRequest();

            try
            {
                var resultFromStorage = await _persistentStorageService.GetByExpressionAsync(x => x.Id == result);
                if (resultFromStorage.FirstOrDefault() == null) return NotFound();
                return Ok(resultFromStorage.FirstOrDefault());
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }


    }
}