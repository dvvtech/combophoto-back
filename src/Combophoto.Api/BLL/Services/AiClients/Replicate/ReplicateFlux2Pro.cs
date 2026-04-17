using Combophoto.Api.BLL.Abstract;
using System.Text;
using System.Text.Json;

namespace Combophoto.Api.BLL.Services.AiClients.Replicate
{
    /// <summary>
    /// https://replicate.com/black-forest-labs/flux-2-pro
    /// </summary>
    public class ReplicateFlux2Pro : IReplicateApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ReplicateFlux2Pro> _logger;
        private readonly IPromptService _promptService;

        public ReplicateFlux2Pro(
            HttpClient httpClient,
            IPromptService promptService,
            ILogger<ReplicateFlux2Pro> logger)
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
                    input_images = imageUrls
                }
            };

            //Serialize request
            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            //Send request            
            var response = await _httpClient.PostAsync("models/black-forest-labs/flux-2-pro/predictions", content);
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
