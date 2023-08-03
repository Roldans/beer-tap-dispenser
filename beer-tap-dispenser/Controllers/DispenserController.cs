using beer_tap_dispenser.Model;
using beer_tap_dispenser.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace beer_tap_dispenser.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DispenserController : ControllerBase
    {
        private readonly IDispenserService _dispenserService;
        private readonly ILogger<DispenserController> _logger;

        public DispenserController(IDispenserService dispenserService, ILogger<DispenserController> logger)
        {
            _dispenserService = dispenserService;
            _logger = logger;
        }

        [HttpGet("usage-info")]
        public IActionResult GetAllDispensersUsageInfo()
        {
            var dispenserUsageInfo = _dispenserService.GetAllDispensersUsageInfo();
            return Ok(dispenserUsageInfo);
        }

        [HttpPost]
        public IActionResult CreateDispenser([FromBody] double flowVolume)
        {
            if (flowVolume <= 0)
            {
                return BadRequest("Flow volume must be a positive value.");
            }

            var createdDispenser = _dispenserService.CreateDispenser(flowVolume);
            _logger.LogInformation($"Created dispenser with Id: {createdDispenser.Id}, FlowVolume: {createdDispenser.FlowVolume}");
            return CreatedAtAction(nameof(CreateDispenser), createdDispenser);
        }

        [HttpPost("{dispenserId}/open")]
        public IActionResult OpenDispenserTap(int dispenserId)
        {
            if (_dispenserService.OpenDispenserTap(dispenserId))
            {
                _logger.LogInformation($"Dispenser with Id {dispenserId} tap opened.");
                return Ok();
            }

            _logger.LogWarning($"Dispenser with Id {dispenserId} not found or tap is already open.");
            return NotFound();
        }

        [HttpPost("{dispenserId}/close")]
        public IActionResult CloseDispenserTap(int dispenserId)
        {
            if (_dispenserService.CloseDispenserTap(dispenserId))
            {
                _logger.LogInformation($"Dispenser with Id {dispenserId} tap closed.");
                return Ok();
            }

            _logger.LogWarning($"Dispenser with Id {dispenserId} not found or tap is already closed.");
            return NotFound();
        }
    }
}
