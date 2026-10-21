using ByShutterBackend.Application;
using ByShutterBackend.Application.Common.Behaviour;
using ByShutterBackend.Domain.ClientAccess;
using ByShutterBackend.Domain.Common.Interfaces;
using ByShutterBackend.Infrastructure.Persistance;
using ByShutterBackend.Infrastructure.Repositories;
using ByShutterBackend.Web.Common;
using ByShutterBackend.Web.Endpoints.ClientAccess;
using ByShutterBackend.Web.Endpoints.HomeConfig;
using ByShutterBackend.Web.Endpoints.Portfolio;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ImageStoragePathOptions>(
    builder.Configuration.GetSection(nameof(ImageStoragePathOptions))
);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "byshutter-session";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddScoped<IHomeProfileRepository, HomeProfileRepository>();
builder.Services.AddScoped<IPortfolioTileRepository, PortfolioTileRepository>();

builder.Services.AddScoped<IAlbumRepository, AlbumRepository>();
builder.Services.AddScoped<IPhotoRepository, PhotoRepository>();
builder.Services.AddScoped<IFileStorage, FileStorage>();


builder.Services.AddDbContext<ByShutterDbContext>(options =>
{
    options.UseNpgsql("Host=postgres;Port=5432;Database=byshutter;Username=byshutter;Password=byshutter_password;");
});


builder.Services.AddMediatR(options =>
{
    options.LicenseKey =
        "eyJhbGciOiJSUzI1NiIsImtpZCI6Ikx1Y2t5UGVubnlTb2Z0d2FyZUxpY2Vuc2VLZXkvYmJiMTNhY2I1OTkwNGQ4OWI0Y2IxYzg1ZjA4OGNjZjkiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2x1Y2t5cGVubnlzb2Z0d2FyZS5jb20iLCJhdWQiOiJMdWNreVBlbm55U29mdHdhcmUiLCJleHAiOiIxODE1NjA5NjAwIiwiaWF0IjoiMTc4NDEzNTI0MyIsImFjY291bnRfaWQiOiIwMTlmNjZiZWFlYjk3NmIwYjdhNTZmMjZlNTgzMDk3MSIsImN1c3RvbWVyX2lkIjoiMDE5ZjY2YmVhZWI5NzZiMGI3YTU2ZjI2ZTU4MzA5NzEiLCJzdWJfaWQiOiItIiwiZWRpdGlvbiI6IjAiLCJ0eXBlIjoiMiJ9.1Eu2a9X4v_eueKPHpH0CMM5CuT1nri69StbH1GmM_F0QFU6xkNH6L3j3kffxDGv6WlUPyLuRz701wMWwLHQ639dota5lOSd1BJJyCiOV-lF6W_v6gS_nBEYkTreYKrODCOV2WZy-i1AoE9DDuJp_W7YrGRRVPmigDoJEJuK5MDHgRJDmD-3rM58Yo1aNukivg7rrvR1Rcb_Sj4K47PXa3-3Kj4WXP1e1pIpmDhcG8BqIishB1iXktOzzrSaoWF8whRScwj_u0u9VhiJauf5nb9Cw-L9KAZdX6WJ_zKQNgwW7H1UcGJbs2tJ-ZVPphbuxcY7BZpcndGkW9C10ClSGhg";
    options.RegisterServicesFromAssembly(typeof(AssemblyReference).Assembly);
});

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));


builder.Services.AddValidatorsFromAssemblyContaining<AssemblyReference>();


builder.Services.AddCors(option =>
{
    option.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins(builder.Configuration["FrontendOrigin"] ?? "http://localhost:5173")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

var app = builder.Build();

// Migrate database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ByShutterDbContext>();
    dbContext.Database.Migrate();

    if (!dbContext.Users.Any())
    {
        var username = app.Configuration["Authentication:InitialUsername"];
        var password = app.Configuration["Authentication:InitialPassword"];

        if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
        {
            var salt = RandomNumberGenerator.GetBytes(32);
            var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 120_000, HashAlgorithmName.SHA256, 32);
            dbContext.Users.Add(new User(username, Convert.ToBase64String(hash), Convert.ToBase64String(salt)));
            dbContext.SaveChanges();
        }
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new { status = "ok", service = nameof(ByShutterBackend) }));

app.MapAuthEndpoints();
app.MapHomeProfileEndpoints();
app.MapPortfolioTilesEndpoints();

app.MapPhotoEndpoints();
app.MapAlbumEndpoints();

app.MapOpenApi();


app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "ByShutterBackend v1");
});

app.Run();