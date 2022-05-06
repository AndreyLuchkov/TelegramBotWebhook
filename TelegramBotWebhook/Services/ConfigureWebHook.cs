using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace TelegramBotWebhook.Services
{
    public class ConfigureWebHook : IHostedService
    {
        IServiceProvider _services;
        BotConfiguration _botConfig;

        public ConfigureWebHook(IConfiguration configuration, IServiceProvider services)
        {
            _services = services;
            _botConfig = configuration.GetSection("BotConfig").Get<BotConfiguration>();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _services.CreateScope();
            var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

            var webhookAddress = @$"{_botConfig.HostName}/bot/{_botConfig.Token}";
            await botClient.SetWebhookAsync(
                url: webhookAddress,
                allowedUpdates: Array.Empty<UpdateType>(),
                cancellationToken: cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            using var scope = _services.CreateScope();
            var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

            await botClient.DeleteWebhookAsync(
                cancellationToken: cancellationToken);
        }
    }
}
