using Combophoto.Api.BLL.Abstract;
using System.Text;
using System.Text.Json;

namespace Combophoto.Api.BLL.Services.AiClients.Replicate
{
    /// <summary>
    /// https://replicate.com/black-forest-labs/flux-2-max
    /// </summary>
    public class ReplicateFlux2MaxApiClient : IReplicateApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ReplicateFlux2MaxApiClient> _logger;
        private readonly IPromptService _promptService;

        public ReplicateFlux2MaxApiClient(
            HttpClient httpClient,
            IPromptService promptService,
            ILogger<ReplicateFlux2MaxApiClient> logger)
        {
            _httpClient = httpClient;
            _promptService = promptService;
            _logger = logger;
        }

        public async Task<string> ProcessPredictionAsync(string[] imageUrls)
        {
            var requestBody = new
            {
                input = new
                {
                    prompt = _promptService.GetPrompt(),
                    input_images = imageUrls,
                    aspect_ratio = "1:1",
                    resolution = "4 MP",
                    output_format = "jpg",
                    output_quality = 100,
                    safety_tolerance = 2
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.PostAsync("models/black-forest-labs/flux-2-max/predictions", content);
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timeout while calling Replicate Flux 2 Max API");
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                var responseContentError = await response.Content.ReadAsStringAsync();
                _logger.LogError(responseContentError);
                _logger.LogInformation("result is null");
                return null;
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            using var document = JsonDocument.Parse(responseContent);
            if (document.RootElement.TryGetProperty("output", out var outputProperty))
            {
                var resultUrl = outputProperty.GetString();
                if (!string.IsNullOrEmpty(resultUrl))
                {
                    _logger.LogInformation("Flux 2 Max result success");
                    return resultUrl;
                }

                _logger.LogInformation("Flux 2 Max result fail");
                _logger.LogError(responseContent);
            }

            return null;
        }
    }
}
