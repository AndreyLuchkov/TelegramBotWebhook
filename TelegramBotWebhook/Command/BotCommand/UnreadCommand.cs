using System.Text;
using TelegramBotWebhook.Services;
using TelegramBot.Web.MPEIEmail;
using TelegramBotWebhook.Web.MPEIEmail.EmailEntities;

namespace TelegramBotWebhook.Command.BotCommand
{
    public class UnreadCommand : BotCommand, IServiceRequired, ISessionDepended
    {
        IEmailReadService? _emailReadService;

        public IEnumerable<Type> RequiredServicesTypes { get; }
        public Session? Session { get; set; }

        internal UnreadCommand() : base("/unread") 
        { 
            RequiredServicesTypes = new Type[1] { typeof(IEmailReadService) };
        }
       
        public void AddService(object service)
        {
            if (service is IEmailReadService emailReadService)
            {
                _emailReadService = emailReadService;
            }
        }
        public override async Task<ExecuteResult> Execute(string option)
        {
            if (Session is null)
                throw new InvalidOperationException("The session property is null.");
            if (Session.UserKey is null)
            {
                return new ExecuteResult(ResultType.Text, "Для использования данной команды необходимо выполнить вход на почту МЭИ.\nВоспользуйтесь командой /login, чтобы войти на почту.");
            }

            var letterRecords = (await _emailReadService!.GetLetters(Session))
                .Where((letter) => !letter.IsRead);

            if (letterRecords.Count() > 0)
            {
                StringBuilder resultMessage = new StringBuilder("Список непрочитанных писем: \n");
                resultMessage.Append(LetterRecordsToString(letterRecords));

                return new ExecuteResult(ResultType.InlineKeyboarUrl, resultMessage, new string[] { "Ссылка на почту МЭИ", $"https://legacy.mpei.ru/CookieAuth.dll?Logon?curl=Z2Fowa&flags=0&forcedownlevel=0&formdir=2&username={Session.Login}&password={Session.Password}&isUtf8=1&trusted=4" }); ;
            }
            else
            {
                return new ExecuteResult(ResultType.Text, "Непрочитанные письма не найдены.");
            }
        }
        private StringBuilder LetterRecordsToString(IEnumerable<LetterRecord> letters)
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
