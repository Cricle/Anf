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
        public const string SchemeName = "Anf";
        public const string AuthenticateHeader = "ANF-TOKEN";

        private readonly UserIdentityService userIdentityService;

        private HttpContext context;
        private bool isApiQuery;

        public AnfAuthenticationHandler(UserIdentityService userIdentityService)
        {
            this.userIdentityService = userIdentityService;
        }

        public async Task<AuthenticateResult> AuthenticateAsync()
        {
            if (isApiQuery)
            {
                var authToken = context.Request.Headers[AuthenticateHeader];
                var authTk = authToken.ToString();
                if (string.IsNullOrEmpty(authTk) || !Guid.TryParse(authTk, out _))
                {
                    if (!context.Request.Cookies.TryGetValue(AuthenticateHeader, out authTk))
                    {
                        return AuthenticateResult.Fail("No authenticate!");
                    }
                }
                var tk = await userIdentityService.GetTokenInfoAsync(authTk);
                if (tk is null)
                {
                    return AuthenticateResult.Fail("No authenticate!");
                }
                context.Features.Set(tk);
                var t = GetAuthTicket(tk.Name);
                return AuthenticateResult.Success(t);
            }
            var tick = GetAuthTicket(string.Empty);
            return AuthenticateResult.Success(tick);
        }
        AuthenticationTicket GetAuthTicket(string name)
        {
            var claimsIdentity = new ClaimsIdentity(new Claim[]
            {
                 new Claim(ClaimTypes.Name, name)
            }, SchemeName);

            var principal = new ClaimsPrincipal(claimsIdentity);
            return new AuthenticationTicket(principal, SchemeName);
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
            isApiQuery = context.Request.Path.Value.StartsWith(AnfConst.ApiPrefx);
            return Task.CompletedTask;
        }
    }
}
