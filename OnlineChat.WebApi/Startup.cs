#nullable enable

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OnlineChat.WebApi.Helpers;
using OnlineChat.WebApi.Models;
using OnlineChat.WebApi.Models.Repos;
using OnlineChat.WebApi.Services;
using OnlineChat.WebApi.Services.InstantMessaging;

namespace OnlineChat.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR()
                    .AddNewtonsoftJsonProtocol(opt =>
                    {
                        opt.PayloadSerializerSettings.TypeNameHandling = TypeNameHandling.Objects;
                        opt.PayloadSerializerSettings.ContractResolver = new DefaultContractResolver();
                    });
            services.AddSingleton<IUserIdProvider, NameUserIdProvider>();
            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.AddDbContext<OnlineChatDatabaseContext>(options =>
            {
                options.UseSqlServer(Configuration["AppSettings:ConnectionString"]);
            });
            
            services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    })
                    .AddJwtBearer(options =>
                    {
                        var key = Configuration.GetSection("AppSettings").GetValue<string>("EncryptionKey");

                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
                            ValidateIssuer = false,
                            ValidateAudience = false
                        };

                        options.Events = new JwtBearerEvents
                        {
                            OnMessageReceived = context =>
                            {
                                var accessToken = context.Request.Query["access_token"];

                                // If the request is for our hub...
                                var path = context.HttpContext.Request.Path;
                                if (!string.IsNullOrEmpty(accessToken) &&
                                    (path.StartsWithSegments("/instantmessaging")))
                                {
                                    // Read the token out of the query string
                                    context.Token = accessToken;
                                }
                                return Task.CompletedTask;
                            }
                        };
                    });

            services.AddTransient<IUserService, UserService>()
                    .AddTransient<IChatService, ChatService>()
                    .AddScoped<ISubscriptionManager, SubscriptionManager>()
                    .AddScoped<IUserRepo, UserRepo>()
                    .AddScoped<IChatRepo, ChatRepo>()
                    .AddScoped<IMessageReadStatusRepo, MessageReadStatusRepo>()
                    .AddScoped<IMessageRepo, MessageRepo>()
                    .AddSingleton(AutomapperConfig.Build());

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseResponseCompression();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseCors(builder =>
            {
                builder.AllowCredentials()
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .SetIsOriginAllowed(origin => true);
            });

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<InstantMessager>("/instantmessaging");
            });
        }
    }
}
