using Telegram.Bot;
using TelegramBotWebhook.Services;

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
            
            services.AddHttpClient("tgwebhook")
                .AddTypedClient<ITelegramBotClient>(httpClient
                    => new TelegramBotClient(BotConfig.Token, httpClient));

            services.AddSingleton<IMessageSendingService<ExecuteResult>, TelegramMessageSendingService>();

            services.AddSingleton<ICommandExecuteService<ExecuteResult>, BotCommandExecuteService>();

            services.AddScoped<UpdateHandleService>();

            services.AddControllers().AddNewtonsoftJson();
        }
    }
}
