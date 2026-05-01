using Combophoto.Api.BLL.Abstract;
using System.Text;
using System.Text.Json;

namespace Combophoto.Api.BLL.Services.AiClients.Replicate
{
    /// <summary>
    /// https://replicate.com/black-forest-labs/flux-2-pro
    /// </summary>
    public class ReplicateFlux2ProApiClient : IReplicateApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ReplicateFlux2ProApiClient> _logger;
        private readonly IPromptService _promptService;

        public ReplicateFlux2ProApiClient(
            HttpClient httpClient,
            IPromptService promptService,
            ILogger<ReplicateFlux2ProApiClient> logger)
        {
            _httpClient = httpClient;
            _promptService = promptService;
            _logger = logger;
        }

        public async Task<string> ProcessPredictionAsync(string[] imageUrls)
        {
            //Create request
            var requestBody = new
            {
                input = new
                {
                    prompt = _promptService.GetPrompt(),
                    input_images = imageUrls,
                    aspect_ratio = "1:1",
                    resolution = "2 MP",
                    output_format = "jpg",
                    output_quality = 100,
                    safety_tolerance = 2
                }
            };

            //Serialize request
            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            //Send request
            HttpResponseMessage response;
            try
            {
                response = await _httpClient.PostAsync("models/black-forest-labs/flux-2-pro/predictions", content);
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timeout while calling Replicate Flux 2 Pro API");
                return null;
            }
            if (!response.IsSuccessStatusCode)
            {
                var responseContentError = await response.Content.ReadAsStringAsync();
                _logger.LogError(responseContentError);
                string httpStatusCode = ((int)response.StatusCode).ToString();
                _logger.LogInformation("result is null");
                return null;
            }

            //Read and parse response
            var responseContent = await response.Content.ReadAsStringAsync();
            using var document = JsonDocument.Parse(responseContent);
            if (document.RootElement.TryGetProperty("output", out var outputProperty))
            {
                var resultUrl = outputProperty.GetString();
                if (!string.IsNullOrEmpty(resultUrl))
                {
                    _logger.LogInformation($"result success");
                    return resultUrl;
                }

                _logger.LogInformation($"result fail");
                _logger.LogError(responseContent);
            }

            return null;
        }
    }
}
