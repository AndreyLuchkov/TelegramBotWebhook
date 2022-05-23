using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotWebhook.Extensions;

namespace TelegramBotWebhook.Services
{
    public class TelegramMessageSendingService : IMessageSendingService<ExecuteResult>
    {
        private readonly ITelegramBotClient botClient;

        private Message? lastMessageWithKeyboadrd;

        public TelegramMessageSendingService(ITelegramBotClient botClient)
        {
            this.botClient = botClient;
        }

        public async Task SendMessage(IChat chat, ExecuteResult options)
        {
            var sender = options.ResultType switch
            {
                ResultType.Text => SendTextMessage(chat.Id, options.Message!, options.RemoveKeyboard),
                ResultType.Keyboard => SendMessageWithKeyboard(chat.Id, options.Message!, options.Values!),
                ResultType.InlineKeyboarUrl => SendMessageWithInlineKeyboardUrl(chat.Id, options.Message!, options.Values!),
                ResultType.InlineKeyboardWithCallback => SendMessageWithInlineKeyboard(chat.Id, options.Message!, options.InlineKeyboardMarkup),
                ResultType.RemoveKeyboard => RemoveKeyboard(chat.Id),
                _ => throw new Exception()
            };
            await sender;
        }
        private async Task SendTextMessage(long chatId, string text, bool removeKeyboard)
        {
            ReplyKeyboardRemove? replyMarkup = null;
            if (removeKeyboard)
                replyMarkup = new ReplyKeyboardRemove();

            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: text ?? "",
                replyMarkup: replyMarkup,
                parseMode: ParseMode.Html);
        }
        private async Task SendMessageWithKeyboard(long chatId, string text, ReplyKeyboardMarkup keyboardMarkup)
        {
            keyboardMarkup.ResizeKeyboard = true;

            lastMessageWithKeyboadrd = await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: text,
            replyMarkup: keyboardMarkup);
        }
        private async Task SendMessageWithInlineKeyboardUrl(long chatId, string text, string[] values)
        {
            InlineKeyboardMarkup inlineKeyboard = new(new[]
            {
                InlineKeyboardButton.WithUrl(
                    text: values[0],
                    url: values[1])
            });

            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: text,
                replyMarkup: inlineKeyboard);
        }
        private async Task SendMessageWithInlineKeyboard(long chatId, string text, InlineKeyboardMarkup? keyboardMarkup)
        {
            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: text,
                replyMarkup: keyboardMarkup);
        }
        private async Task RemoveKeyboard(long chatId)
        {
            if (lastMessageWithKeyboadrd is not null)
            {
                await botClient.EditMessageReplyMarkupAsync(
                    chatId: chatId,
                    messageId: lastMessageWithKeyboadrd.MessageId);
            }
        }
    }
}
