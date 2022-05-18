using System.Text;
using TelegramBotWebhook.Services;
using TelegramBotWebhook.Web.MPEIEmail.EmailEntities;

namespace TelegramBotWebhook.Command.BotCommand
{
    public sealed class LessonsCommand : SessionedBotCommand
    {
        private IEmailReadService? _emailReadService;
        private IEmailLetterReadService<LessonLetter>? _emailLetterReadService;

        internal LessonsCommand() : base("/lessons")
        {
            AddRequiredServiceTypes(new[] { typeof(IEmailReadService), typeof(IEmailLetterReadService<LessonLetter>) });
        }

        protected override void AddNewService(object service)
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
        protected override async Task<ExecuteResult> ConcreteExecute(string option)
        {
            if (Session!.UserKey is null)
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
                    .AppendLine(lessonLetter.SessionNumber.ToString())
                    .AppendLine(lessonLetter.SessionPassword);
            }

            return resultMessage;
        }
    }
}
