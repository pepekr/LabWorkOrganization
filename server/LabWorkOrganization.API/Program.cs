using DotNetEnv;
using LabWorkOrganization.API;
using LabWorkOrganization.Application.Dtos;
using LabWorkOrganization.Application.Interfaces;
using LabWorkOrganization.Domain.Utilities;
using LabWorkOrganization.Infrastructure.Data.ExternalAPIs.Clients;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

Env.Load();

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);


builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient<JWKSClient>(client =>
{
    client.BaseAddress = new Uri("https://www.googleapis.com/oauth2/v3/");
});

builder.Services.AddAppDI(builder.Configuration);
builder.Services.AddAuthorization();

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
                    string? cookieToken = context.Request.Cookies["access_token"];
                    if (!string.IsNullOrEmpty(cookieToken))
                    {
                        context.Token = cookieToken;
                        Console.WriteLine("[AUTH TRACE] 🍪 Token pulled from cookie.");
                    }
                    else
                    {
                        Console.WriteLine("[AUTH TRACE] ⚠️ No token found in header or cookie.");
                    }
                }
                else
                {
                    Console.WriteLine("[AUTH TRACE] 🧠 Token found in Authorization header.");
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


WebApplication app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<LabWorkOrganization.Infrastructure.Data.AppDbContext>();
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

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

            // 3️⃣ Set new cookies
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

            // 4️⃣ Inject new access token into the request headers for downstream middleware
            context.Request.Headers["Authorization"] = $"Bearer {newAccess}";
        }
        else
        {
            Console.WriteLine($"[AUTH REFRESH] ❌ Token refresh failed: {refreshResult.ErrorMessage}");
        }
    }

    // 5️⃣ Continue request pipeline
    await next();
});

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
