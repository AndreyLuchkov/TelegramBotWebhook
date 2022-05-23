using System.Text;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotWebhook
{
    public enum ResultType
    {
        Text,
        Keyboard,
        InlineKeyboarUrl,
        InlineKeyboardWithCallback,
        EditMessageWithInlineKeyboard,
        RemoveKeyboard,
        NoEdit,
    }
    public class ExecuteResult
    {
        public ResultType ResultType { get; init; }
        public string? Message { get; set; }
        public string[]? Values { get; set; }
        public bool RemoveKeyboard { get; set; } = true;
        public ReplyKeyboardMarkup? ReplyKeyboardMarkup { get; set; }
        public InlineKeyboardMarkup? InlineKeyboardMarkup { get; set; }

        public ExecuteResult(ResultType resultType)
        {
            ResultType = resultType;
        }
        public ExecuteResult(ResultType resultType, string? message) : this(resultType)
        {
            Message = message;
        }
        public ExecuteResult(ResultType resultType, string? message, string[]? values) : this(resultType, message)
        {
            Values = values;
        }
        public ExecuteResult(ResultType resultType, StringBuilder? message) : this(resultType)
        {
            Message = message?.ToString();
        }
        public ExecuteResult(ResultType resultType, StringBuilder? message, string[]? values) : this(resultType)
        {
            Message = message?.ToString();
            Values = values;
        }
    }
}
