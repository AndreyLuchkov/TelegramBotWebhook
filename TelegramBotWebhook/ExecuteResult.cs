using System.Text;

namespace TelegramBotWebhook
{
    public enum ResultType
    {
        Text,
        Keyboard,
        InlineKeyboarUrl,
        RemoveKeyboard,
    }
    public class ExecuteResult
    {
        public ResultType ResultType { get; init; }
        public string? Message { get; set; }
        public string[]? Values { get; set; }
        public bool RemoveKeyboard { get; set; }

        public ExecuteResult(ResultType resultType, bool removeKeyboard = true)
        {
            ResultType = resultType;
            RemoveKeyboard = removeKeyboard;
        }
        public ExecuteResult(ResultType resultType, string? message, bool removeKeyboard = true) : this(resultType, removeKeyboard)
        {
            Message = message;
        }
        public ExecuteResult(ResultType resultType, string? message, string[]? values, bool removeKeyboard = true) : this(resultType, message, removeKeyboard)
        {
            Values = values;
        }
        public ExecuteResult(ResultType resultType, StringBuilder? message, bool removeKeyboard = true) : this(resultType, removeKeyboard)
        {
            Message = message?.ToString();
        }
        public ExecuteResult(ResultType resultType, StringBuilder? message, string[]? values, bool removeKeyboard = true) : this(resultType, removeKeyboard)
        {
            Message = message?.ToString();
            Values = values;
        }
    }
}
