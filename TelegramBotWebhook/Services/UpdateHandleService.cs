using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotWebhook.Extensions;

namespace TelegramBotWebhook.Services
{
    public class UpdateHandleService 
    {
        private readonly ITelegramBotClient botClient;
        private readonly ILogger<UpdateHandleService> logger;
        private readonly ICommandExecuteService<ExecuteResult> executeService;
        private readonly IMessageSendingService<ExecuteResult> messageSendingService;

        public UpdateHandleService(ITelegramBotClient botClient, ILogger<UpdateHandleService> logger, ICommandExecuteService<ExecuteResult> executeService, IMessageSendingService<ExecuteResult> messageSendingService)
        {
            this.botClient = botClient;
            this.logger = logger;
            this.executeService = executeService;
            this.messageSendingService = messageSendingService;
        }

        public async Task HandleUpdateAsync(Update update)
        {
            var user = update.Message!.From!;
            string messageText = update.Message.Text!;

            if (update.Message.Type != MessageType.Text)
            {
                logger.LogInformation($"Get a not text message from {user.FirstName} {user.LastName}(ID:{user.Id}) {update.Message.Date}.");
                return;
            }

            var splitedText = messageText.Split(' ');

            ExecuteResult result;
            if (splitedText.First().Contains('/') && splitedText.Count() == 1)
            {
                logger.LogInformation($"Get the command {splitedText.First()} from {user.FirstName} {user.LastName}(ID:{user.Id}) {update.Message.Date}.");

                result = executeService.ExecuteCommand(splitedText.First()).Result;
            }
            else if (executeService.ExecuteIsOver())
            {
                logger.LogInformation($"Get a text message from {user.FirstName} {user.LastName}(ID:{user.Id}) {update.Message.Date}.");

                result = executeService.ExecuteCommand("/start").Result;
            }
            else
            {
                result = executeService.HandleResponse(messageText).Result;
            }

            Chat chat = new Chat(update.Message.Chat.Id);
            await messageSendingService.SendMessage(chat, result);
        }
    }
}
