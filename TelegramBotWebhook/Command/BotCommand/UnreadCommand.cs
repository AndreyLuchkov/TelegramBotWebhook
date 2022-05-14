using System.Text;
using TelegramBotWebhook.Services;
using TelegramBot.Web.MPEIEmail;
using TelegramBotWebhook.MPEIEmail.EmailEntities;

namespace TelegramBotWebhook.Command.BotCommand
{
    internal class UnreadCommand : BotCommand, IServiceRequired, ISessionDepended
    {
        private List<object> _services;
        public IEnumerable<Type> RequiredServicesTypes { get; }
        public Session? Session { get; set; }

        internal UnreadCommand() : base("/unread") 
        { 
            RequiredServicesTypes = new Type[1] { typeof(IEmailReadService) };
            _services = new List<object>(1);
        }
       
        public void AddService(object service)
        {
            if (_services.Count() == 0)
                _services.Add(service);
        }
        public override async Task<ExecuteResult> Execute(string option)
        {
            if (Session is null)
                throw new InvalidOperationException("The session property is null.");
            if (Session.Login is null || Session.Password is null)
            {
                return new ExecuteResult(ResultType.Text, "Для использования данной команды необходима аутентификация. Воспользуйтесь командой /login, чтобы иметь доступ к почте МЭИ.");
            }

            var emailReadService = (IEmailReadService)_services.First();

            var letters = (await emailReadService.GetLetters(Session))
                .Where((letter) => !letter.IsRead);

            if (letters.Count() > 0)
            {
                StringBuilder resultMessage = new StringBuilder("Список непрочитанных писем: \n");
                resultMessage.Append(LettersToString(letters));

                return new ExecuteResult(ResultType.InlineKeyboarUrl, resultMessage, new string[] { "Ссылка на почту МЭИ", $"https://legacy.mpei.ru/CookieAuth.dll?Logon?curl=Z2Fowa&flags=0&forcedownlevel=0&formdir=2&username={Session.Login}&password={Session.Password}&isUtf8=1&trusted=4" }); ;
            }
            else
            {
                return new ExecuteResult(ResultType.Text, "Непрочитанные письма не найдены.");
            }
        }
        private StringBuilder LettersToString(IEnumerable<LetterRecord> letters)
        {
            StringBuilder lettersInfo = new StringBuilder();

            foreach (var letter in letters)
            {
                lettersInfo.AppendLine(LetterRecord.FromTrimDots(letter.From))
                    .AppendLine($"{letter.Theme} \n{letter.ReceivingTime.ToString("g")}\n");
            }

            return lettersInfo;
        }
    }
}
