namespace Combophoto.Api.BLL.Services.AiClients.Replicate
{
    public class ReplicateNanoBanana2ApiClient
    {
        private readonly HttpClient _httpClient;

        private readonly ILogger<ReplicateNanoBanana2ApiClient> _logger;
    
        public ReplicateNanoBanana2ApiClient(
            HttpClient httpClient,
            ILogger<ReplicateNanoBanana2ApiClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }
    }
}
