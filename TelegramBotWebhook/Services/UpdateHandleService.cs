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

        private int lastMessageWithKeyboadrdId = -1;

        public UpdateHandleService(ITelegramBotClient botClient, ILogger<UpdateHandleService> logger, ICommandExecuteService<ExecuteResult> executeService)
        {
            this.botClient = botClient;
            this.logger = logger;
            this.executeService = executeService;
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

            await HandlerDispatcher(update.Message, result);

        }

        private async Task HandlerDispatcher(Message message, ExecuteResult result)
        {
            var handler = result.ResultType switch
            {
                ResultType.Text => SendTextMessage(message, result.Message!),
                ResultType.Keyboard => SendMessageWithKeyboard(message, result.Message!, result.Values!),
                ResultType.Empty => Task.CompletedTask,
                ResultType.RemoveKeyboard => RemoveKeyboard(message),
                _ => throw new Exception()
            };
            await handler;
        } 
        private async Task SendTextMessage(Message message, string text)
        {
            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: text ?? "",
                replyMarkup: new ReplyKeyboardRemove());
        }
        private async Task SendMessageWithKeyboard(Message message, string text, string[] values)
        {
            var buttons = values.Select((text) => new KeyboardButton(text));

            var replyKeyboardMarkup = new ReplyKeyboardMarkup(buttons!.Split(2))
            {
                ResizeKeyboard = true
            };

            var sentMessage = await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: text,
            replyMarkup: replyKeyboardMarkup);

            lastMessageWithKeyboadrdId = sentMessage.MessageId;
        }
        private async Task RemoveKeyboard(Message message)
        {
            if (lastMessageWithKeyboadrdId != -1)
            {
                await botClient.EditMessageReplyMarkupAsync(
                    chatId: message.Chat.Id,
                    messageId: lastMessageWithKeyboadrdId);
            }
        } 
    }
}
