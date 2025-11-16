using DotNetEnv;
using LabWorkOrganization.API;
using LabWorkOrganization.Application.Dtos;
using LabWorkOrganization.Application.Interfaces;
using LabWorkOrganization.Domain.Utilities;
using LabWorkOrganization.Infrastructure.Data.ExternalAPIs.Clients;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using LabWorkOrganization.API.Configurations;

// Load environment variables from the .env file
Env.Load();

// Initialize the WebApplication builder
var builder = WebApplication.CreateBuilder(args);

// Register essential services for dependency injection
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddEndpointsApiExplorer();

// Configure HttpClient for external API (Google OAuth)
builder.Services.AddHttpClient<JWKSClient>(client =>
{
    client.BaseAddress = new Uri("https://www.googleapis.com/oauth2/v3/");
});

// Register application-level dependency injection
builder.Services.AddAppDI(builder.Configuration);
builder.Services.AddAuthorization();

// Configure Swagger with JWT authentication support
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter JWT token manually here (for Swagger testing only)"
        });
});
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

// Configure JWT Authentication with Bearer tokens
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.RequireHttpsMetadata = true;
        o.SaveToken = false;

        // Configure JWT event handlers
        o.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                string? token = context.Token ?? context.Request.Cookies["access_token"];
                if (!string.IsNullOrEmpty(token))
                {
                    context.Token = token;
                    Console.WriteLine("[AUTH TRACE] 🍪 Token pulled from cookie or header.");
                }
                else
                {
                    Console.WriteLine("[AUTH TRACE] ⚠️ No token found in header or cookie.");
                }

                Console.WriteLine($"[AUTH TRACE] Raw Token: {context.Token ?? "NULL"}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("[AUTH TRACE] ✅ Token successfully validated.");
                Console.WriteLine($"[AUTH TRACE] User: {context.Principal?.Identity?.Name}");
                Console.WriteLine($"[AUTH TRACE] Claims: {context.Principal?.Claims.Count()}");
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"[AUTH TRACE] ❌ Authentication failed: {context.Exception?.Message}");
                return Task.CompletedTask;
            }
        };

        // Token validation parameters
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
            ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET")!)
            ),
            ClockSkew = TimeSpan.Zero
        };
    });

// Build the app
var app = builder.Build();

var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

// Enable Swagger in development mode
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        foreach (var desc in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/{desc.GroupName}/swagger.json", desc.GroupName.ToUpperInvariant());
        }
    });
}

// Enforce HTTPS redirection
app.UseHttpsRedirection();

// Configure CORS policy to allow Angular frontend requests
app.UseCors(c => c
    .WithOrigins("http://localhost:4200", "https://localhost:4200", "https://localhost:7220")
    .AllowCredentials()
    .AllowAnyHeader()
    .AllowAnyMethod()
);

// Middleware to refresh JWT tokens if access token is missing
app.Use(async (context, next) =>
{
    string? accessToken = context.Request.Cookies["access_token"]
                          ?? context.Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");

    string? refreshToken = context.Request.Cookies["refresh_token"]
                           ?? context.Request.Headers["X-Refresh-Token"].FirstOrDefault();

    if (string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken))
    {
        Console.WriteLine("[AUTH REFRESH] 🌀 Attempting token refresh...");

        var authService = context.RequestServices.GetRequiredService<IAuthService>();
        Result<JWTTokenDto> refreshResult = await authService.HandleRefresh(refreshToken);

        if (refreshResult.IsSuccess && refreshResult.Data is not null)
        {
            string newAccess = refreshResult.Data.AccessToken;
            string newRefresh = refreshResult.Data.RefreshToken;

            // Set new cookies
            context.Response.Cookies.Append("access_token", newAccess, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddMinutes(60)
            });

            context.Response.Cookies.Append("refresh_token", newRefresh, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(10)
            });

            // Inject new access token into request headers
            context.Request.Headers["Authorization"] = $"Bearer {newAccess}";
            Console.WriteLine("[AUTH REFRESH] ✅ Tokens refreshed successfully.");
        }
        else
        {
            Console.WriteLine($"[AUTH REFRESH] ❌ Token refresh failed: {refreshResult.ErrorMessage}");
        }
    }

    await next();
});

// Enable authentication & authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Map all controllers
app.MapControllers();

// Run the application
app.Run();
