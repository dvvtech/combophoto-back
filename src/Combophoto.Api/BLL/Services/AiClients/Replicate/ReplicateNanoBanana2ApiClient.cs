using Combophoto.Api.BLL.Abstract;
using System.Text;
using System.Text.Json;

namespace Combophoto.Api.BLL.Services.AiClients.Replicate
{
    public class ReplicateNanoBanana2ApiClient : IReplicateNanoBanana2ApiClient
    {
        private readonly HttpClient _httpClient;

        private readonly ILogger<ReplicateNanoBanana2ApiClient> _logger;
        private readonly IPromptService _promptService;
    
        public ReplicateNanoBanana2ApiClient(
            HttpClient httpClient,
            IPromptService promptService,
            ILogger<ReplicateNanoBanana2ApiClient> logger)
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
                    //ompt = "In the first photo, there is a person. In the second photo, there is a hairstyle of another person. Transfer the hairstyle from the second photo onto the head of the person in the first photo. Keep the face, pose, and background from the first photo, but make the hair the same as in the second photo (shape, length, color, style). The result should look as realistic and natural as possible.",
                    image_input = imageUrls                    
                }
            };

            //Serialize request
            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            //Send request            
            var response = await _httpClient.PostAsync("models/google/nano-banana-2/predictions", content);
            if (!response.IsSuccessStatusCode)
            {
                var responseContentError = await response.Content.ReadAsStringAsync();
                _logger.LogError(responseContentError);
                string httpStatusCode = ((int)response.StatusCode).ToString();
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
                    return resultUrl;
                }

                _logger.LogError(responseContent);
            }

            return null;
        }
    }
}
