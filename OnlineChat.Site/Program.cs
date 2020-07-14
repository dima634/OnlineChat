using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.Authorization;
using OnlineChat.Site.Auth;
using MatBlazor;
using OnlineChat.Site.WebApi;
using OnlineChat.Site.InstantMessaging;
using System.Net.Http;
using Microsoft.AspNetCore.SignalR.Protocol;

namespace OnlineChat.Site
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddTransient(factory => new HttpClient());
            builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
            builder.Services.AddSingleton<WebApiClient>();
            builder.Services.AddSingleton<InstantMessager>();
            builder.Services.AddOptions();
            builder.Services.AddAuthorizationCore();
            builder.Services.AddMatToaster(config =>
            {
                config.Position = MatToastPosition.TopRight;
                config.PreventDuplicates = true;
                config.NewestOnTop = true;
                config.ShowCloseButton = true;
                config.MaximumOpacity = 100;
                config.VisibleStateDuration = 3000;
            });
            builder.Services.AddBlazorContextMenu();
            builder.Services.AddSingleton<IHubProtocol, NewtonsoftJsonHubProtocol>(sp => new NewtonsoftJsonHubProtocol());

            await builder.Build().RunAsync();
        }
    }
}
