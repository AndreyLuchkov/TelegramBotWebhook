using TelegramBot.Web.MPEIEmail;

namespace TelegramBotWebhook.Command.BotCommand
{
    internal class LoginCommand : BotCommand, ILongRunning, ISessionDepended
    {
        public Session? Session { get; set; }
        internal LoginCommand() : base("/login") { }

        public event Action? ExecuteIsOver;

        public override Task<ExecuteResult> Execute(string option)
        {
            if (Session is null)
                throw new InvalidOperationException("The session property is null.");

            if (Session.Login is null && option == String.Empty)
            {
                return Task.FromResult(new ExecuteResult(ResultType.Text, "Введите логин для почты:"));
            } 
            else if (Session.Login is null && option != String.Empty)
            {
                Session.Login = option;
                return Task.FromResult(new ExecuteResult(ResultType.Text, "Введите пароль:"));
            }
            else
            {
                Session.Password = option;

                ExecuteIsOver?.Invoke(); 
                return Task.FromResult(new ExecuteResult(ResultType.RemoveKeyboard));
            }
        }
    }
}
