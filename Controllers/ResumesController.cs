using JobSearchPortal_KeyWords.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JobSearchPortal_KeyWords.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResumesController : ControllerBase
    {
        private readonly ILogger<ResumesController> _logger;
        private readonly IElasticService _elasticService;   

        public ResumesController(ILogger<ResumesController> logger, IElasticService elasticService )
        {
            _logger = logger;
            _elasticService = elasticService;
        }

        [HttpPost ("create-index")]
        public async Task<IActionResult> CreateIndexAsync(string indexName)
        {
            try
            {
                await _elasticService.CreateIndexIfNotExistsAsync(indexName);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create index");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

    }
}
