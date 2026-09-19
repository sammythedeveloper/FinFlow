using FinancialTracker.API.Data;
using FinancialTracker.API.Middleware;
using FinancialTracker.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

builder.Services.AddControllers();
builder.Services.AddScoped<IAuthService, AuthService>();

// ========== SUPABASE JWT AUTH ==========
var supabaseUrl = builder.Configuration["Supabase:Url"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"{supabaseUrl}/auth/v1";

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = $"{supabaseUrl}/auth/v1",

            ValidateAudience = true,
            ValidAudience = "authenticated",

            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ClockSkew = TimeSpan.FromMinutes(1)
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("===== JWT AUTH FAILED =====");
                Console.WriteLine(context.Exception.ToString());
                Console.WriteLine("===========================");
                return Task.CompletedTask;
            }
        };
    });
    
// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("VueDev", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173",
                "https://finflow-client-one.vercel.app"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Apply EF Core migrations automatically
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors("VueDev");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();