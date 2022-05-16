using System.Text;
using TelegramBot.Web.MPEIEmail;
using TelegramBotWebhook.Web.MPEIEmail.EmailEntities;
using TelegramBotWebhook.Services;

namespace TelegramBotWebhook.Command.BotCommand
{
    public class LessonsCommand : BotCommand, IServiceRequired, ISessionDepended
    {
        IEmailReadService? _emailReadService;
        IEmailLetterReadService<LessonLetter>? _emailLetterReadService;

        public IEnumerable<Type> RequiredServicesTypes { get; }
        public Session? Session { get; set; }

        internal LessonsCommand() : base("/lessons")
        {
            RequiredServicesTypes = new Type[2] { typeof(IEmailReadService), typeof(IEmailLetterReadService<LessonLetter>) };
        }

        public void AddService(object service)
        {
            if (service is IEmailReadService emailReadService)
            {
                _emailReadService = emailReadService;
            }
            else if (service is IEmailLetterReadService<LessonLetter> emailLetterReadService)
            {
                _emailLetterReadService = emailLetterReadService;
            }
        }
        public override async Task<ExecuteResult> Execute(string option)
        {
            if (Session is null)
                throw new InvalidOperationException("The session property is null.");
            if (_emailReadService is null || _emailLetterReadService is null)
                throw new InvalidOperationException("The required services has not added.");

            if (Session.UserKey is null)
            {
                return new ExecuteResult(ResultType.Text, "Для использования данной команды необходимо выполнить вход на почту МЭИ.\nВоспользуйтесь командой /login, чтобы войти на почту.");
            }

            var letterRecords = _emailReadService!.GetLetters(Session);

            var lessonLetters = _emailLetterReadService!.ReadLetters(
                Session, 
                (await letterRecords)
                    .Where((letter) => letter.Type == "IPM.Schedule.Meeting.Request"));

            return new ExecuteResult(ResultType.Text, MakeResultMessage(await lessonLetters));
        }
        private StringBuilder MakeResultMessage(IEnumerable<LessonLetter> lessonLetters)
        {
            StringBuilder resultMessage = new StringBuilder("Письма с приглашением на занятие: \n");

            foreach (var lessonLetter in lessonLetters)
            {
                resultMessage.AppendLine(lessonLetter.Teacher)
                    .AppendLine(lessonLetter.LessonStartDate.ToString())
                    .AppendLine(lessonLetter.SessionLink)
                    .AppendLine();
            }

            return resultMessage;
        }
    }
}
