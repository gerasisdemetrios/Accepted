using CSharpApp.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
builder.Logging.ClearProviders().AddSerilog(logger);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.Configure<HttpClientSettings>(builder.Configuration.GetSection("appsettings"));
builder.Services.Configure<RestApiSettings>(builder.Configuration.GetSection("appsettings"));
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

// Register CategoriesService HttpClient
builder.Services.AddHttpClient<ICategoriesService, CategoriesService>(HttpClientHelper.ConfigureHttpClient)
    .AddHttpMessageHandler<JwtAuthorizationHandler>()
    .SetHandlerLifetime(httpClientLifetime)
    .AddPolicyHandler((serviceProvider, request) => HttpClientHelper.GetRetryPolicy(serviceProvider));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMiddleware<RequestTimingMiddleware>();
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

#region Products

versionedEndpointRouteBuilder.MapGet("api/v{version:apiVersion}/products", async (IProductsService productsService) =>
    {
        var products = await productsService.GetProducts();
        return Results.Ok(products);
    }).WithName("GetProducts")
 .HasApiVersion(1.0)
 .WithOpenApi();

versionedEndpointRouteBuilder.MapGet("api/v{version:apiVersion}/products/{id:int}", async (int id, IProductsService productsService) =>
{
    var product = await productsService.GetProduct(id);
    return Results.Ok(product);
}).WithName("GetProductById")
.HasApiVersion(1.0)
.WithOpenApi();

versionedEndpointRouteBuilder.MapPost("api/v{version:apiVersion}/products", async (Product product, IProductsService productsService) =>
{
    await productsService.CreateProduct(product);
    return Results.NoContent();
}).WithName("CreateProduct")
.HasApiVersion(1.0)
.WithOpenApi();

versionedEndpointRouteBuilder.MapPut("api/v{version:apiVersion}/products/{id:int}", async (int id, Product product, IProductsService productsService) =>
{
    await productsService.UpdateProduct(id, product);
    return Results.NoContent();
}).WithName("UpdateProduct")
.HasApiVersion(1.0)
.WithOpenApi();

versionedEndpointRouteBuilder.MapDelete("api/v{version:apiVersion}/products/{id:int}", async (int id, IProductsService productsService) =>
{
    await productsService.DeleteProduct(id);
    return Results.NoContent();
}).WithName("DeleteProduct")
.HasApiVersion(1.0)
.WithOpenApi();

#endregion

#region Categories

versionedEndpointRouteBuilder.MapGet("api/v{version:apiVersion}/categories", async (ICategoriesService productsService) =>
{
    var categories = await productsService.GetCategories();
    return Results.Ok(categories);
}).WithName("GetCategories")
 .HasApiVersion(1.0)
 .WithOpenApi();


versionedEndpointRouteBuilder.MapGet("api/v{version:apiVersion}/categories/{id:int}", async (int id, ICategoriesService categoriesService) =>
{
    var category = await categoriesService.GetCategory(id);
    return Results.Ok(category);
}).WithName("GetContactById")
.HasApiVersion(1.0)
.WithOpenApi();

versionedEndpointRouteBuilder.MapPost("api/v{version:apiVersion}/categories", async (Category category, ICategoriesService categoriesService) =>
{
    await categoriesService.CreateCategory(category);
    return Results.NoContent();
}).WithName("CreateCategory")
.HasApiVersion(1.0)
.WithOpenApi();

versionedEndpointRouteBuilder.MapPut("api/v{version:apiVersion}/categories/{id:int}", async (int id, Category category, ICategoriesService categoriesService) =>
{
    await categoriesService.UpdateCategory(id, category);
    return Results.NoContent();
}).WithName("UpdateCategory")
.HasApiVersion(1.0)
.WithOpenApi();

versionedEndpointRouteBuilder.MapDelete("api/v{version:apiVersion}/categories/{id:int}", async (int id, ICategoriesService categoriesService) =>
{
    await categoriesService.DeleteCategory(id);
    return Results.NoContent();
}).WithName("DeleteCategory")
.HasApiVersion(1.0)
.WithOpenApi();

#endregion

app.Run();