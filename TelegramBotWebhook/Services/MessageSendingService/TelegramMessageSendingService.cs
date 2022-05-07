using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotWebhook.Extensions;

namespace TelegramBotWebhook.Services
{
    public class TelegramMessageSendingService : IMessageSendingService<ExecuteResult>
    {
        private readonly ITelegramBotClient botClient;

        private int lastMessageWithKeyboadrdId = -1;

        public TelegramMessageSendingService(ITelegramBotClient botClient)
        {
            this.botClient = botClient;
        }

        public async Task SendMessage(IChat chat, ExecuteResult options)
        {
            var sender = options.ResultType switch
            {
                ResultType.Text => SendTextMessage(chat.Id, options.Message!),
                ResultType.Keyboard => SendMessageWithKeyboard(chat.Id, options.Message!, options.Values!),
                ResultType.Empty => Task.CompletedTask,
                ResultType.RemoveKeyboard => RemoveKeyboard(chat.Id),
                _ => throw new Exception()
            };
            await sender;
        }
        private async Task SendTextMessage(long chatId, string text)
        {
            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: text ?? "",
                replyMarkup: new ReplyKeyboardRemove());
        }
        private async Task SendMessageWithKeyboard(long chatId, string text, string[] values)
        {
            var buttons = values.Select((text) => new KeyboardButton(text));

            var replyKeyboardMarkup = new ReplyKeyboardMarkup(buttons!.Split(2))
            {
                ResizeKeyboard = true
            };

            var sentMessage = await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: text,
            replyMarkup: replyKeyboardMarkup);

            lastMessageWithKeyboadrdId = sentMessage.MessageId;
        }
        private async Task RemoveKeyboard(long chatId)
        {
            if (lastMessageWithKeyboadrdId != -1)
            {
                await botClient.EditMessageReplyMarkupAsync(
                    chatId: chatId,
                    messageId: lastMessageWithKeyboadrdId);
            }
        }
    }
}
