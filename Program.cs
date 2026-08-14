using FinancialTracker.API.Services;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    
    // Connects Swagger UI to .NET's native OpenAPI endpoint
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "FinancialTracker API v1");
        options.RoutePrefix = string.Empty; // Loads Swagger directly at the root URL
    });
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();