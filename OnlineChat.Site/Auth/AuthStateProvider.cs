using OnlineChat.Site.WebApi;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using OnlineChat.Dtos.BindingModels;

namespace OnlineChat.Site.Auth
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        private WebApiClient _api;
        private ClaimsIdentity _identity;

        public string Username => _identity.Name;

        public AuthStateProvider(WebApiClient webApi)
        {
            _api = webApi;
        }
        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var user = new ClaimsPrincipal(_identity ?? new ClaimsIdentity());

            return Task.FromResult(new AuthenticationState(user));
        }

        public async Task Login(AuthorizeModel authorizeModel)
        {
            _identity = await _api.Login(authorizeModel.Username, authorizeModel.Password);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
            return;
        }

        public void Logoff()
        {
            _identity = null;
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}
