using Autofac.Core;
using CSharpApp.Api.Handler;
using CSharpApp.Api.Helper;
using CSharpApp.Application.Communication;
using CSharpApp.Application.Products;
using CSharpApp.Core.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
builder.Logging.ClearProviders().AddSerilog(logger);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.Configure<HttpClientSettings>(builder.Configuration.GetSection("appsettings"));
builder.Services.Configure<RestApiSettings>(builder.Configuration.GetSection("appsettings"));
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();
builder.Services.AddTransient<JwtAuthorizationHandler>();
builder.Services.AddDefaultConfiguration();
builder.Services.AddHttpConfiguration();
builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApiVersioning().AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

var httpClientLifetime = TimeSpan.FromMinutes(builder.Configuration.GetValue<int>("HttpClientSettings:Lifetime"));

// Register JwtTokenService HttpClient
builder.Services.AddHttpClient<IJwtTokenService, JwtTokenService>(HttpClientHelper.ConfigureHttpClient)
    .SetHandlerLifetime(httpClientLifetime)
    .AddPolicyHandler((serviceProvider, request) => HttpClientHelper.GetRetryPolicy(serviceProvider));

// Register ProductsService HttpClient
builder.Services.AddHttpClient<IProductsService, ProductsService>(HttpClientHelper.ConfigureHttpClient)
    .AddHttpMessageHandler<JwtAuthorizationHandler>()
    .SetHandlerLifetime(httpClientLifetime)
    .AddPolicyHandler((serviceProvider, request) => HttpClientHelper.GetRetryPolicy(serviceProvider));

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

versionedEndpointRouteBuilder.MapGet("api/v{version:apiVersion}/products", async (IProductsService productsService) =>
    {
        var products = await productsService.GetProducts();
        return products;
    })
    .WithName("Products")
    .HasApiVersion(1.0)
    .WithOpenApi();

versionedEndpointRouteBuilder.MapGet("api/v{version:apiVersion}/products/{id:int}", async (int id, IProductsService productsService) =>
{
    var products = await productsService.GetProduct(id);
    return products;
})
    .WithName("Product")
    .HasApiVersion(1.0)
    .WithOpenApi();

app.Run();