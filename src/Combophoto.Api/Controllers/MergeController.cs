using Combophoto.Api.BLL.Abstract;
using Combophoto.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Combophoto.Api.Controllers
{
    [Route("merge")]
    [ApiController]
    public class MergeController : ControllerBase
    {
        private readonly IReplicateApiClient _apiClient;
        private readonly ILogger<MergeController> _logger;

        public MergeController(
            IReplicateApiClient apiClient,
            ILogger<MergeController> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        [HttpPost("run")]
        public async Task<ActionResult> Run(CombophotoRequest request)
        {
            var resUrl = await _apiClient.ProcessPredictionAsync(request.ImageUrls);
            if (string.IsNullOrEmpty(resUrl))
            {
                return BadRequest();
            }

            return Ok(resUrl);
        }

        [HttpGet("test")]
        public async Task<ActionResult> Test()
        {
            try
            {
                _logger.LogInformation("start");

                var baseUrl = "http://pyproject-api:8080";

                using var httpClient = new HttpClient();

                var response = await httpClient.GetAsync($"{baseUrl}/health");
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation(content);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("error1111111111111111");
            }

            return Ok("12345");
        }
    }
}
