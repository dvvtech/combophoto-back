using Combophoto.Api.BLL.Abstract;
using Combophoto.Api.BLL.Services;
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

            InitConfigs();
            ConfigureServices();

            _builder.Services.AddControllers();
        }

        private void InitConfigs()
        {
            _builder.Services.Configure<S3Config>(_builder.Configuration.GetSection(S3Config.SectionName));
            _builder.Services.Configure<ReplicateConfig>(_builder.Configuration.GetSection(ReplicateConfig.SectionName));
        }

        private void ConfigureServices()
        {            
            _builder.Services.AddScoped<IPromptService, PromptService>();
            _builder.Services.AddScoped<IStorageService, S3StorageService>();
        }
    }
}
