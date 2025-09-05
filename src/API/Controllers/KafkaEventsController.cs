using Microsoft.AspNetCore.Mvc;
using TQM.Backoffice.Application.Responses;
using TQM.BackOffice.Persistence.Services;

namespace TQM.BackOffice.API.Controllers;

[ApiController]
[Route("[controller]")]
public class KafkaEventsController : ControllerBase
{
    private readonly KafkaConsumerService _kafkaConsumerService;

    public KafkaEventsController(KafkaConsumerService kafkaConsumerService)
    {
        _kafkaConsumerService = kafkaConsumerService;
    }

    [HttpGet("GetRecentEvents")]
    public async Task<ActionResult<BaseResponse<object>>> GetRecentEvents()
    {
        try
        {
            var events = _kafkaConsumerService.GetRecentEvents();
            
            return Ok(new BaseResponse<object>
            {
                Success = true,
                Message = "Recent Kafka events retrieved successfully",
                ResponseObject = events
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new BaseResponse<object>
            {
                Success = false,
                Message = $"Error retrieving Kafka events: {ex.Message}"
            });
        }
    }

    [HttpGet("GetEventStats")]
    public async Task<ActionResult<BaseResponse<object>>> GetEventStats()
    {
        try
        {
            var stats = _kafkaConsumerService.GetEventStatistics();
            
            return Ok(new BaseResponse<object>
            {
                Success = true,
                Message = "Kafka event statistics retrieved successfully",
                ResponseObject = stats
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new BaseResponse<object>
            {
                Success = false,
                Message = $"Error retrieving event statistics: {ex.Message}"
            });
        }
    }

    [HttpPost("ClearEventLogs")]
    public async Task<ActionResult<BaseResponse<object>>> ClearEventLogs()
    {
        try
        {
            _kafkaConsumerService.ClearEventLogs();
            
            return Ok(new BaseResponse<object>
            {
                Success = true,
                Message = "Event logs cleared successfully"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new BaseResponse<object>
            {
                Success = false,
                Message = $"Error clearing event logs: {ex.Message}"
            });
        }
    }
}
