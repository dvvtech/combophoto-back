using Combophoto.Api.AppStart.Extensions;
using Combophoto.Api.BLL.Abstract;
using Combophoto.Api.BLL.Services;
using Combophoto.Api.BLL.Services.AiClients.FaceSwap;
using Combophoto.Api.BLL.Services.AiClients.Replicate;
using Combophoto.Api.BLL.Services.S3;
using Combophoto.Api.Configuration;

namespace Combophoto.Api.AppStart
{
    public class Startup
    {
        private WebApplicationBuilder _builder;

        public Startup(WebApplicationBuilder builder)
        {
            _builder = builder ?? throw new ArgumentNullException(nameof(builder));
        }

        public void Initialize()
        {
            if (_builder.Environment.IsDevelopment())
            {
                _builder.Services.AddSwaggerGen();
            }
            else
            {
                _builder.Services.ConfigureCors();
            }

            InitConfigs();
            ConfigureServices();
            ConfigureClientAPI();

            _builder.Services.AddControllers();
        }

        private void InitConfigs()
        {
            if (!_builder.Environment.IsDevelopment())
            {
                _builder.Configuration.AddKeyPerFile("/run/secrets", optional: true);
            }

            _builder.Services.Configure<S3CloudConfig>(_builder.Configuration.GetSection(S3CloudConfig.SectionName));
            _builder.Services.Configure<ReplicateConfig>(_builder.Configuration.GetSection(ReplicateConfig.SectionName));
            _builder.Services.Configure<FaceSwapConfig>(_builder.Configuration.GetSection(FaceSwapConfig.SectionName));
        }

        private void ConfigureServices()
        {            
            _builder.Services.AddScoped<IPromptService, PromptService>();
            _builder.Services.AddScoped<IStorageService, S3StorageService>();
        }

        private void ConfigureClientAPI()
        {
            //_builder.Services.AddHttpClient<IReplicateNanoBanana2ApiClient, ReplicateNanoBanana2ApiClient>((serviceProvider, client) =>
            _builder.Services.AddHttpClient<IReplicateApiClient, ReplicateFlux2MaxApiClient>((serviceProvider, client) =>
            {
                var replicateConfig = _builder.Configuration.GetSection(ReplicateConfig.SectionName).Get<ReplicateConfig>();

                client.BaseAddress = new Uri("https://api.replicate.com/v1/");
                client.Timeout = TimeSpan.FromSeconds(90);
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {replicateConfig.ApiToken}");
                client.DefaultRequestHeaders.Add("Prefer", "wait");
            });

            _builder.Services.AddHttpClient<IFaceSwapApiClient, FaceSwapApiClient>((serviceProvider, client) =>
            {
                var faceSwapConfig = _builder.Configuration.GetSection(FaceSwapConfig.SectionName).Get<FaceSwapConfig>()
                    ?? new FaceSwapConfig();

                client.BaseAddress = new Uri(faceSwapConfig.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(faceSwapConfig.TimeoutSeconds);
            });
        }
    }
}
