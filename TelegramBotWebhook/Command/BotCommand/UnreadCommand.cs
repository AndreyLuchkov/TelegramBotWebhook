using System.Text;
using TelegramBotWebhook.Services;
using TelegramBot.Web.MPEIEmail;
using TelegramBotWebhook.MPEIEmail.EmailEntities;

namespace TelegramBotWebhook.Command.BotCommand
{
    public class UnreadCommand : BotCommand, IServiceRequired
    {
        private List<object> _services;
        public IEnumerable<Type> RequiredServicesTypes { get; }

        public UnreadCommand() : base("/unread") 
        { 
            RequiredServicesTypes = new Type[1] { typeof(IEmailReadService) };
            _services = new List<object>(1);
        }
       
        public void AddService(object service)
        {
            if (_services.Count() == 0)
            {
                _services.Add(service);
            }
        }
        public override Task<ExecuteResult> Execute(string option)
        {
            var emailReadService = (IEmailReadService)_services.First();
            
            var letters = emailReadService.ReadLetters().Result.Where((letter) => !letter.IsRead);

            if (letters.Count() > 0)
            {
                StringBuilder resultMessage = new StringBuilder("The list of your unread letters: \n");
                resultMessage.Append(LettersToString(letters));

                Session session = Session.Instance();

                var result = new ExecuteResult(ResultType.InlineKeyboarUrl, resultMessage, new string[] { "Link to the MPEI Email", $"https://legacy.mpei.ru/CookieAuth.dll?Logon?curl=Z2Fowa&flags=0&forcedownlevel=0&formdir=2&username={session.Login}&password={session.Password}&isUtf8=1&trusted=4" });

                return Task.FromResult(result);
            }
            else
            {
                return Task.FromResult(new ExecuteResult(ResultType.Text, "Unread letters not found."));
            }
        }
        private StringBuilder LettersToString(IEnumerable<Letter> letters)
        {
            StringBuilder lettersInfo = new StringBuilder();

            foreach (var letter in letters)
            {
                lettersInfo.AppendLine(Letter.FromTrimDots(letter.From));
                lettersInfo.AppendLine($"{letter.Theme} \n{letter.ReceivingTime.ToString("g")}\n");
            }

            return lettersInfo;
        }
    }
}
