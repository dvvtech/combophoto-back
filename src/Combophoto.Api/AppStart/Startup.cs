using Combophoto.Api.BLL.Abstract;
using Combophoto.Api.BLL.Services;

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
            ConfigureServices();

            _builder.Services.AddControllers();
        }

        private void ConfigureServices()
        {            
            _builder.Services.AddScoped<IPromptService, PromptService>();            
        }
    }
}
