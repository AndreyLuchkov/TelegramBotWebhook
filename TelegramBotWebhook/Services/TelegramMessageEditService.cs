using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotWebhook.Services
{
    public class TelegramMessageEditService
    {
        private readonly ITelegramBotClient _botClient;

        public TelegramMessageEditService(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        public async Task EditMessage(Message message, ExecuteResult options)
        {
            var sender = options.ResultType switch
            {
                ResultType.EditMessageWithInlineKeyboard => EditMessageWithInlineKeyboard(message.Chat.Id, message.MessageId, options.Message, options.InlineKeyboardMarkup),
                ResultType.InlineKeyboardWithCallback => EditMessageWithInlineKeyboard(message.Chat.Id, message.MessageId, options.Message, options.InlineKeyboardMarkup),
                ResultType.NoEdit => Task.CompletedTask,
                _ => throw new Exception()
            };
            await sender;
        }
        private async Task EditMessageWithInlineKeyboard(long chatId, int messageId, string? text, InlineKeyboardMarkup? keyboardMarkup)
        {
            await _botClient.EditMessageTextAsync(
                chatId: new ChatId(chatId),
                messageId: messageId,
                text: text ?? " ",
                replyMarkup: keyboardMarkup,
                parseMode: ParseMode.Html);
        }
    }
}
