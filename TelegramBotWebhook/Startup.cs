using Telegram.Bot;
using TelegramBotWebhook.Web.MPEIEmail.EmailEntities;
using TelegramBotWebhook.Services;
using TelegramBotWebhook.Web;

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
                .AddTypedClient <IPollingClient, MPEIEmailPollingClient>();

            services.AddHttpClient("tgwebhook")
                .AddTypedClient<ITelegramBotClient>(httpClient
                    => new TelegramBotClient(BotConfig.Token, httpClient));

            services.AddScoped<ISessionService, MPEIEmailSessionService>();

            services.AddSingleton<HttpFactories>();

            services.AddSingleton<HttpWorker>();

            services.AddTransient<IEmailAutentificationService, MPEIEmailAutentificationService>();

            services.AddTransient<IEmailReadService, MPEIEmailReadService>();

            services.AddTransient<IEmailLetterReadService<LessonLetter>, LessonLetterReadService>();

            services.AddSingleton<IMessageSendingService<ExecuteResult>, TelegramMessageSendingService>();

            services.AddSingleton<ICommandExecuteService<ExecuteResult>, BotCommandExecuteService>();

            services.AddScoped<UpdateHandleService>();

            services.AddControllers().AddNewtonsoftJson();
        }
    }
}
