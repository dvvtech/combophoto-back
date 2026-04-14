using Combophoto.Api.BLL.Abstract;
using Combophoto.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Combophoto.Api.Controllers
{
    [Route("merge")]
    [ApiController]
    public class MergeController : ControllerBase
    {
        private readonly IReplicateNanoBanana2ApiClient _apiClient;

        public MergeController(IReplicateNanoBanana2ApiClient apiClient)
        {
            _apiClient = apiClient;
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
        public ActionResult Test()
        { 
            return Ok("12345");
        }
    }
}
