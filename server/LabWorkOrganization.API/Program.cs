using DotNetEnv;
using LabWorkOrganization.API;
using LabWorkOrganization.Application.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using System.Text;

// Load environment variables from the .env file
Env.Load();

// Initialize the WebApplication builder
var builder = WebApplication.CreateBuilder(args);

// Register essential services for dependency injection
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure HttpClient for external API (Google OAuth)
builder.Services.AddHttpClient<LabWorkOrganization.Infrastructure.Data.ExternalAPIs.Clients.JWKSClient>(client =>
{
    client.BaseAddress = new Uri("https://www.googleapis.com/oauth2/v3/");
});

// Register application-level dependency injection
builder.Services.AddAppDI(builder.Configuration);
builder.Services.AddAuthorization();

// Configure Swagger with JWT authentication support
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token manually here (for Swagger testing only)"
    });
});

// Configure JWT Authentication with Bearer tokens
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        // Require HTTPS and do not save tokens on the server
        o.RequireHttpsMetadata = true;
        o.SaveToken = false;

        // 🔍 Configure JWT event handlers for better debugging and token tracing
        o.Events = new JwtBearerEvents
        {
            // Extract token from cookies if missing in header
            OnMessageReceived = context =>
            {
                if (string.IsNullOrEmpty(context.Token))
                {
                    var cookieToken = context.Request.Cookies["access_token"];
                    if (!string.IsNullOrEmpty(cookieToken))
                    {
                        context.Token = cookieToken;
                        Console.WriteLine($"[AUTH TRACE] 🍪 Token pulled from cookie.");
                    }
                    else
                    {
                        Console.WriteLine($"[AUTH TRACE] ⚠️ No token found in header or cookie.");
                    }
                }
                else
                {
                    Console.WriteLine($"[AUTH TRACE] 🧠 Token found in Authorization header.");
                }

                Console.WriteLine($"[AUTH TRACE] Raw Token: {context.Token ?? "NULL"}");
                return Task.CompletedTask;
            },

            // Log successful validation
            OnTokenValidated = context =>
            {
                Console.WriteLine($"[AUTH TRACE] ✅ Token successfully validated.");
                Console.WriteLine($"[AUTH TRACE] User: {context.Principal?.Identity?.Name}");
                Console.WriteLine($"[AUTH TRACE] Claims: {context.Principal?.Claims.Count()}");
                return Task.CompletedTask;
            },

            // Log authentication failures
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"[AUTH TRACE] ❌ Authentication failed: {context.Exception?.Message}");
                return Task.CompletedTask;
            }
        };

        // 🔑 Token validation parameters (issuer, audience, secret)
        o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
            ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET")!)
            ),
            ClockSkew = TimeSpan.Zero // No time tolerance for expired tokens
        };
    });

// Build the app
var app = builder.Build();

// Enable Swagger in development mode
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
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

// Custom middleware to handle token refresh using refresh_token cookie
app.Use(async (context, next) =>
{
    var accessToken = context.Request.Cookies["access_token"];
    var refreshToken = context.Request.Cookies["refresh_token"];

    // Only refresh if access token is missing or expired, but refresh token exists
    if (string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken))
    {
        Console.WriteLine("[AUTH REFRESH] 🌀 Attempting token refresh...");

        var refreshService = context.RequestServices.GetRequiredService<IAuthService>();
        var refreshResult = await refreshService.HandleRefresh(refreshToken);

        if (refreshResult.IsSuccess && refreshResult.Data is not null)
        {
            var newAccess = refreshResult.Data.AccessToken;
            var newRefresh = refreshResult.Data.RefreshToken;

            // Set new cookies for updated tokens
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

            Console.WriteLine("[AUTH REFRESH] ✅ Tokens refreshed successfully.");
            // Inject new access token into the request header
            context.Request.Headers["Authorization"] = $"Bearer {newAccess}";
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
