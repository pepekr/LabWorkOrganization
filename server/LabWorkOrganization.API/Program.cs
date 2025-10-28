using DotNetEnv;
using LabWorkOrganization.API;
using LabWorkOrganization.Application.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using System.Text;

Env.Load();

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient<LabWorkOrganization.Infrastructure.Data.ExternalAPIs.Clients.JWKSClient>(client =>
{
    client.BaseAddress = new Uri("https://www.googleapis.com/oauth2/v3/");
});

builder.Services.AddAppDI(builder.Configuration);
builder.Services.AddAuthorization();

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
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {

        o.RequireHttpsMetadata = true;
        o.SaveToken = false;


        o.Events = new JwtBearerEvents
        {
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
            OnTokenValidated = context =>
            {
                Console.WriteLine($"[AUTH TRACE] ✅ Token successfully validated.");
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
            ClockSkew = TimeSpan.Zero
        };
    });


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(c => c
    .WithOrigins("http://localhost:4200", "https://localhost:4200", "https://localhost:7220")
    .AllowCredentials()
    .AllowAnyHeader()
    .AllowAnyMethod()
);
app.Use(async (context, next) =>
{
    var accessToken = context.Request.Cookies["access_token"];
    var refreshToken = context.Request.Cookies["refresh_token"];

    // Only refresh if access token is missing or expired, and we have a refresh token
    if (string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken))
    {
        Console.WriteLine("[AUTH REFRESH] 🌀 Attempting token refresh...");

        var refreshService = context.RequestServices.GetRequiredService<IAuthService>(); // whatever service has HandleRefresh
        var refreshResult = await refreshService.HandleRefresh(refreshToken);

        if (refreshResult.IsSuccess && refreshResult.Data is not null)
        {
            var newAccess = refreshResult.Data.AccessToken;
            var newRefresh = refreshResult.Data.RefreshToken;

            // Set new cookies
            context.Response.Cookies.Append("access_token", newAccess, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None, // adjust if needed (Strict/Lax/None)
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
            // inject new token into the context for JwtBearer to pick up
            context.Request.Headers["Authorization"] = $"Bearer {newAccess}";
        }
        else
        {
            Console.WriteLine($"[AUTH REFRESH] ❌ Token refresh failed: {refreshResult.ErrorMessage}");
        }
    }

    await next();
});

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
