using Combophoto.Api.BLL.Abstract;

namespace Combophoto.Api.BLL.Services.AiClients.Replicate
{
    public class ReplicateNanoBanana2ApiClient
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
    }
}
