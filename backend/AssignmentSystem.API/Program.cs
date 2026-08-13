using System.Text;
using AssignmentSystem.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using AssignmentSystem.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// ── Database ──────────────────────────────────────────────────────────────────
// Tell EF Core to use PostgreSQL with our connection string from appsettings.json
var rawUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
string connectionString;

if (rawUrl != null)
{
    var uri = new Uri(rawUrl);
    var userInfo = uri.UserInfo.Split(':');
    connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";
}
else
{
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));
// Read JWT config from appsettings.json
var jwtSecret = builder.Configuration["JwtSettings:Secret"]!;
var jwtIssuer = builder.Configuration["JwtSettings:Issuer"]!;
var jwtAudience = builder.Configuration["JwtSettings:Audience"]!;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
    };
});

builder.Services.AddAuthorization();
// Register TokenService so controllers can use it via dependency injection
builder.Services.AddScoped<TokenService>();

// ── Controllers ───────────────────────────────────────────────────────────────
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// ── CORS ──────────────────────────────────────────────────────────────────────
// Allow the Next.js frontend to talk to this API
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",          // local dev
                "https://your-vercel-app.vercel.app" // update this after deploying
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// ── Swagger ───────────────────────────────────────────────────────────────────
// Swagger is the auto-generated API docs UI at /swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Assignment System API",
        Version = "v1"
    });

    // This lets you send JWT tokens from the Swagger UI for testing
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token here"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// ── Auto-migrate and seed on startup ─────────────────────────────────────────
// This runs migrations and seeds demo data every time the app starts
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();       // creates tables if they don't exist
    await DbSeeder.SeedAsync(db);           // inserts demo accounts if empty
}

// ── Middleware pipeline ───────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("FrontendPolicy");
app.UseAuthentication();    // must come before UseAuthorization
app.UseAuthorization();
app.MapControllers();

app.Run();