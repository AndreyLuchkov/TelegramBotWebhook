using Telegram.Bot;
using TelegramBotWebhook.Services;
using TelegramBotWebhook.Web;
using TelegramBotWebhook.Web.HttpFactories;

namespace TelegramBotWebhook
{
    public class Startup 
    {
        public IConfiguration Configuration { get; }
        private BotConfiguration BotConfig { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            BotConfig = configuration.GetSection("BotConfig").Get<BotConfiguration>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseCors();

            app.UseEndpoints(endpoints =>
            {
                var token = BotConfig.Token;
                endpoints.MapControllerRoute(name: "tgwebhook",
                                             pattern: $"bot/{token}",
                                             new { controller = "Bot", action = "WebhookPost" });
                endpoints.MapControllers();
            });
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHostedService<ConfigureWebHook>();

            services.AddHttpClient("MPEIEmail")
                .AddTypedClient<IPollingClient, MPEIEmailPollingClient>();

            services.AddHttpClient("tgwebhook")
                .AddTypedClient<ITelegramBotClient>(httpClient
                    => new TelegramBotClient(BotConfig.Token, httpClient));

            services.AddScoped<IHttpFactory, LoginHttpFactory>();

            services.AddScoped<IEmailReadService, MPEIEmailReadService>();

            services.AddSingleton<IMessageSendingService<ExecuteResult>, TelegramMessageSendingService>();

            services.AddScoped<ICommandExecuteService<ExecuteResult>, BotCommandExecuteService>();

            services.AddScoped<UpdateHandleService>();

            services.AddControllers().AddNewtonsoftJson();
        }
    }
}
