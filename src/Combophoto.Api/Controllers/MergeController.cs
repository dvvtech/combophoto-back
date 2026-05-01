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
        private readonly IFaceSwapApiClient _faceSwapApiClient;
        private readonly ILogger<MergeController> _logger;

        public MergeController(
            IReplicateApiClient apiClient,
            IFaceSwapApiClient faceSwapApiClient,
            ILogger<MergeController> logger)
        {
            _apiClient = apiClient;
            _faceSwapApiClient = faceSwapApiClient;
            _logger = logger;
        }

        [HttpPost("run")]
        public async Task<ActionResult> Run(CombophotoRequest request, CancellationToken cancellationToken)
        {
            if (request.ImageUrls == null || request.ImageUrls.Length < 2)
            {
                return BadRequest("At least two image URLs are required.");
            }

            var resUrl = await _apiClient.ProcessPredictionAsync(request.ImageUrls);
            if (string.IsNullOrEmpty(resUrl))
            {
                return BadRequest();
            }

            var faceSwapResultUrl = await _faceSwapApiClient.SwapFacesAsync(
                resUrl,
                request.ImageUrls,
                cancellationToken);

            if (string.IsNullOrEmpty(faceSwapResultUrl))
            {
                _logger.LogError("Face swap failed for generated image {GeneratedImageUrl}", resUrl);
                return StatusCode(StatusCodes.Status502BadGateway, "Face swap failed.");
            }

            return Ok(faceSwapResultUrl);
        }

        //[HttpGet("test")]
        //public async Task<ActionResult> Test()
        //{
        //    try
        //    {
        //        _logger.LogInformation("start");

        //        var baseUrl = "http://pyproject-api:8080";

        //        using var httpClient = new HttpClient();

        //        var response = await httpClient.GetAsync($"{baseUrl}/health");
        //        var content = await response.Content.ReadAsStringAsync();
        //        _logger.LogInformation(content);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogInformation("error1111111111111111");
        //    }

        //    return Ok("12345");
        //}

        //https://api.cloud-platform.pro/combophoto/merge/test

        [HttpGet("test")]
        public async Task<ActionResult> Test()
        {
            try
            {
                _logger.LogInformation("start");

                var baseUrl = "http://faceswap-api:8080";

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
