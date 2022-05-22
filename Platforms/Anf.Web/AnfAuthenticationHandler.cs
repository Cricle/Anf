using Anf.WebService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Anf.Web
{
    internal class AnfAuthenticationHandler : IAuthenticationHandler
    {
        private static readonly PathString ApiPrefxPath = new PathString(AnfConst.ApiPrefx.TrimEnd('/'));


        private readonly UserIdentityService userIdentityService;

        private HttpContext context;
        private bool isApiQuery;

        public AnfAuthenticationHandler(UserIdentityService userIdentityService)
        {
            this.userIdentityService = userIdentityService;
        }

        public async Task<AuthenticateResult> AuthenticateAsync()
        {
            var authToken = context.Request.Headers[AuthenticationConst.AuthHeader];
            var authTk = authToken.ToString();
            if (isApiQuery && (string.IsNullOrEmpty(authTk) || !Guid.TryParse(authTk, out _)))
            {
                if (!context.Request.Cookies.TryGetValue(AuthenticationConst.AuthHeader, out authTk))
                {
                    return AuthenticateResult.Fail("No authenticate!");
                }
            }
            var tk = await userIdentityService.GetTokenInfoAsync(authTk);
            if (tk is null)
            {
                if (isApiQuery)
                {
                    return AuthenticateResult.Fail("No authenticate!");
                }
                return AuthenticateResult.Success(GetAuthTicket(string.Empty));
            }
            context.Features.Set(tk);
            var t = GetAuthTicket(tk.Name);
            return AuthenticateResult.Success(t);
        }
        public static AuthenticationTicket GetAuthTicket(string name)
        {
            var claimsIdentity = new ClaimsIdentity(new Claim[]
            {
                 new Claim(ClaimTypes.Name, name)
            }, AuthenticationConst.SchemeName);

            var principal = new ClaimsPrincipal(claimsIdentity);
            return new AuthenticationTicket(principal, AuthenticationConst.SchemeName);
        }
        public Task ChallengeAsync(AuthenticationProperties properties)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return Task.CompletedTask;
        }

        public Task ForbidAsync(AuthenticationProperties properties)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return Task.CompletedTask;
        }
        public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
        {
            this.context = context;
            isApiQuery = context.Request.Path.StartsWithSegments(ApiPrefxPath);
            return Task.CompletedTask;
        }
    }
}
