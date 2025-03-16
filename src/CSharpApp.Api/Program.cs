using CSharpApp.Api.Helper;
using CSharpApp.Core.Settings;
using Microsoft.Extensions.Options;
var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
builder.Logging.ClearProviders().AddSerilog(logger);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.Configure<HttpClientSettings>(builder.Configuration.GetSection("appsettings"));
builder.Services.Configure<RestApiSettings>(builder.Configuration.GetSection("appsettings"));
builder.Services.AddDefaultConfiguration();
builder.Services.AddHttpConfiguration();
builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApiVersioning().AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});
builder.Services.AddHttpClient("HTTPClient", (serviceProvider, client) =>
{
    var httpClientSettings = serviceProvider.GetRequiredService<IOptions<HttpClientSettings>>().Value;
    var restApiSettings = serviceProvider.GetRequiredService<IOptions<RestApiSettings>>().Value;
    client.BaseAddress = new Uri(restApiSettings.BaseUrl!);

}).SetHandlerLifetime(TimeSpan.FromMinutes(builder.Configuration.GetValue<int>("HttpClientSettings:Lifetime")))
    .AddPolicyHandler((serviceProvider, request) =>
{
    var settings = serviceProvider.GetRequiredService<IOptions<HttpClientSettings>>().Value;
    return HttpClientHelper.GetRetryPolicy(settings.RetryCount, settings.SleepDuration);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/openapi/v1.json", "OpenAPI v1");
        c.RoutePrefix = string.Empty;
    });

    app.UseReDoc(c =>
    {
        c.SpecUrl("/openapi/v1.json");
    });
}

//app.UseHttpsRedirection();

var versionedEndpointRouteBuilder = app.NewVersionedApi();

versionedEndpointRouteBuilder.MapGet("api/v{version:apiVersion}/getproducts", async (IProductsService productsService) =>
    {
        var products = await productsService.GetProducts();
        return products;
    })
    .WithName("GetProducts")
    .HasApiVersion(1.0)
    .WithOpenApi();

app.Run();