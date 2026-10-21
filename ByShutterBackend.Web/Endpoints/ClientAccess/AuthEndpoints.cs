using System.Security.Claims;
using System.Security.Cryptography;
using ByShutterBackend.Domain.ClientAccess;
using ByShutterBackend.Infrastructure.Persistance;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace ByShutterBackend.Web.Endpoints.ClientAccess;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth");

        group.MapPost("/login", LoginAsync).AllowAnonymous();
        group.MapPost("/logout", async (HttpContext context) =>
        {
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Results.NoContent();
        }).RequireAuthorization();
        group.MapGet("/me", (ClaimsPrincipal user) => Results.Ok(new { username = user.Identity?.Name }))
            .RequireAuthorization();

        return app;
    }

    private static async Task<IResult> LoginAsync(
        LoginRequest request,
        ByShutterDbContext db,
        HttpContext httpContext,
        CancellationToken ct)
    {
        var user = await db.Users.SingleOrDefaultAsync(candidate => candidate.Username == request.Username, ct);

        if (user is null || !PasswordMatches(request.Password, user.PasswordHash, user.PasswordSalt))
        {
            return Results.Unauthorized();
        }

        var claims = new[] { new Claim(ClaimTypes.Name, user.Username) };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await httpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity));

        return Results.NoContent();
    }

    private static bool PasswordMatches(string password, string encodedHash, string encodedSalt)
    {
        var salt = Convert.FromBase64String(encodedSalt);
        var expectedHash = Convert.FromBase64String(encodedHash);
        var actualHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 120_000, HashAlgorithmName.SHA256, expectedHash.Length);
        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }

    public sealed record LoginRequest(string Username, string Password);
}
