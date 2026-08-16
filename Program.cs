using FinancialTracker.API.Data;
using FinancialTracker.API.Middleware; // Namespace for your exception middleware
using FinancialTracker.API.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Register AppDbContext with PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddScoped<IAuthService, AuthService>();

// NOTE: Ensure your JWT Authentication services (e.g., builder.Services.AddAuthentication(...) ) 
// are configured above this line if you haven't already!

var app = builder.Build();

// Configure the HTTP request pipeline.

// 1. Global Exception Handling goes FIRST so it catches errors from everything below it
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    
    // Connects Swagger UI to .NET's native OpenAPI
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "FinancialTracker API v1");
        options.RoutePrefix = string.Empty; 
    });
}

app.UseHttpsRedirection();

// 2. Critical for JWT [Authorize] attributes to work
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();