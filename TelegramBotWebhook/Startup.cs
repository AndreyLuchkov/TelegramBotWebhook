using Telegram.Bot;
using TelegramBotWebhook.Web.MPEIEmail.EmailEntities;
using TelegramBotWebhook.Services;
using TelegramBotWebhook.Web;
using Microsoft.EntityFrameworkCore;

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

            services.AddDbContext<MPEISessionDbContext>(options
                => options.UseSqlServer(Configuration.GetConnectionString("SqlServer")));

            services.AddHttpClient("MPEIEmail")
                .AddTypedClient <IPollingClient, MPEIEmailPollingClient>();

            services.AddHttpClient("tgwebhook")
                .AddTypedClient<ITelegramBotClient>(httpClient
                    => new TelegramBotClient(BotConfig.Token, httpClient));
            
            services.AddSingleton<IMessageSendingService<ExecuteResult>, TelegramMessageSendingService>();

            services.AddSingleton<TelegramMessageEditService>();

            services.AddSingleton<LongRunningCommandSaver>();

            services.AddTransient<HttpFactories>();

            services.AddTransient<HttpWorker>();

            services.AddTransient<IEmailAutentificationService, MPEIEmailAutentificationService>();

            services.AddTransient<IEmailReadService, MPEIEmailReadService>();

            services.AddTransient<IEmailLetterReadService<LessonLetter>, LessonLetterReadService>();

            services.AddScoped<ICommandExecuteService<ExecuteResult>, BotCommandExecuteService>();

            services.AddScoped<ISessionService<MPEISession>, MPEISession.MPEISessionService>();

            services.AddScoped<UpdateCallBackQueryHandleService>();

            services.AddScoped<UpdateMessageHandleService>();

            services.AddControllers().AddNewtonsoftJson();
        }
    }
}
