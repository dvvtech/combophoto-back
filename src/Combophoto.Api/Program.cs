using Combophoto.Api.AppStart;
using Combophoto.Api.AppStart.Extensions;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting Combophoto API");

    var builder = WebApplication.CreateBuilder(args);

    var startup = new Startup(builder);
    startup.Initialize();

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    if (builder.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    else
    {
        app.ApplyCors();
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Combophoto API terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
