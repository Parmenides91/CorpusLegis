using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace CorpusLegis.Web.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/")
            .WithTags("Authentication");

        group.MapGet("/login", Login);
        group.MapPost("/logout", Logout);
    }



    private static async Task Login(HttpContext context)
    {
        var returnUrl = context.Request.Query["returnUrl"].FirstOrDefault();
        var redirect = string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl;
        var props = new AuthenticationProperties { RedirectUri = redirect };
        await context.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme, props);
    }

    private static async Task Logout(HttpContext context)
    {
        var props = new AuthenticationProperties { RedirectUri = "/" };
        await context.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme, props);
        await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }

}
