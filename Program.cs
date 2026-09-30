using FinancialTracker.API.Data;
using FinancialTracker.API.Models;
using FinancialTracker.API.Middleware;
using FinancialTracker.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
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
// Apply EF Core migrations automatically and add demo data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    db.Database.Migrate();

    // Add safe demo data for the Azure assignment
    if (!db.Users.Any(u => u.Email == "demo@finflow.local"))
    {
        var demoUser = new User
        {
            Id = Guid.NewGuid(),
            Username = "Demo User",
            Email = "demo@finflow.local",
            CreatedAt = DateTime.UtcNow
        };

        db.Users.Add(demoUser);
        db.SaveChanges();

        var food = new Category
        {
            Name = "Food",
            Type = "Expense",
            UserId = demoUser.Id
        };

        var transportation = new Category
        {
            Name = "Transportation",
            Type = "Expense",
            UserId = demoUser.Id
        };

        var salary = new Category
        {
            Name = "Salary",
            Type = "Income",
            UserId = demoUser.Id
        };

        db.Categories.AddRange(food, transportation, salary);
        db.SaveChanges();

        var transactions = new List<Transaction>
        {
            new Transaction
            {
                Amount = 25.50m,
                Description = "Lunch",
                Date = DateTime.UtcNow.AddDays(-2),
                UserId = demoUser.Id,
                CategoryId = food.Id
            },
            new Transaction
            {
                Amount = 45.00m,
                Description = "Transit",
                Date = DateTime.UtcNow.AddDays(-1),
                UserId = demoUser.Id,
                CategoryId = transportation.Id
            },
            new Transaction
            {
                Amount = 2500.00m,
                Description = "Monthly salary",
                Date = DateTime.UtcNow,
                UserId = demoUser.Id,
                CategoryId = salary.Id
            }
        };

        db.Transactions.AddRange(transactions);
        db.SaveChanges();
    }
}

// Middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors("VueDev");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();