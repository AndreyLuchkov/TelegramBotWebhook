using System.Text;

namespace TelegramBotWebhook
{
    public enum ResultType
    {
        Empty,
        Text,
        Keyboard,
        RemoveKeyboard,
    }
    public class ExecuteResult
    {
        public ResultType ResultType { get; init; }
        public string? Message { get; set; }
        public string[]? Values { get; set; }

        public ExecuteResult(ResultType resultType)
        {
            ResultType = resultType;
        }
        public ExecuteResult(ResultType resultType, string? message) : this(resultType)
        {
            Message = message;
        }
        public ExecuteResult(ResultType resultType, StringBuilder? message) : this(resultType)
        {
            Message = message?.ToString();
        }
        public ExecuteResult(ResultType resultType, string? message, string[]? values) : this(resultType, message)
        {
            Values = values;
        }
    }
}
