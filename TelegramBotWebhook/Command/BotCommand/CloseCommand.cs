using System.Diagnostics;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotWebhook.Extensions;

namespace TelegramBotWebhook.Command.BotCommand
{
    public class CloseCommand : BotCommand, ILongRunning
    {
        private readonly Dictionary<string, string> _allowedProcessesToClose;
        string? processToCloseName;

        public event Action? ExecuteIsOver;

        internal CloseCommand() : base("close") 
        {
            _allowedProcessesToClose = new Dictionary<string, string>
            {
                ["CiscoCollabHost"] = "Cisco Desktop",
                ["atmgr"] = "Cisco Web",
            };
        }

        public override async Task<ExecuteResult> Execute(string option)
        {
            if (IsUserId(option))
            {
                string[] processesToClose = FindProcessesToClose();

                if (processesToClose.Length == 0)
                {
                    return new ExecuteResult(ResultType.Text, "Доступных для закрытия процессов не найдено.");
                }
                else
                {
                    return new ExecuteResult(ResultType.Keyboard, "Выберите процесс, который хотите закрыть.")
                    {
                        ReplyKeyboardMarkup = new ReplyKeyboardMarkup(processesToClose.Split(3)
                                                                                        .Select(textRow => textRow.Select(text => new KeyboardButton(text))))
                    };
                }
            }
            else if (option != String.Empty && processToCloseName is null)
            {
                if (_allowedProcessesToClose.ContainsValue(option))
                {
                    processToCloseName = _allowedProcessesToClose.Where((pair) => pair.Value == option)
                                                                    .Select((pair) => pair.Key).First();
                    return new ExecuteResult(ResultType.Keyboard, $"Вы действительно хотите закрыть процесс {option}?") 
                    { 
                        ReplyKeyboardMarkup = new ReplyKeyboardMarkup(new[] { new KeyboardButton("Да"), new KeyboardButton("Нет") })
                    };
                }
                else
                {
                    return new ExecuteResult(ResultType.Text, "Неверное имя процесса.");
                }
            }
            else
            {
                if (option.ToLower() == "да")
                {
                    Process[] processes = Process.GetProcessesByName(processToCloseName);

                    if (processes.Length == 0)
                    {
                        ExecuteIsOver?.Invoke();
                        return new ExecuteResult(ResultType.Text, $"Процесс {_allowedProcessesToClose[processToCloseName!]} уже закрыт.");
                    }
                    else
                    {
                        await KillAllProcesses(processes);
                        ExecuteIsOver?.Invoke();
                        return new ExecuteResult(ResultType.Text, $"Процесс {_allowedProcessesToClose[processToCloseName!]} успешно закрыт.");
                    }
                } 
                else
                {
                    ExecuteIsOver?.Invoke();
                    return new ExecuteResult(ResultType.RemoveKeyboard);
                }
            }
        }
        private bool IsUserId(string option) => long.TryParse(option, out long userId);
        private string[] FindProcessesToClose()
        {
            var currentProcesses = Process.GetProcesses().AsParallel()
                .Select((process) => process.ProcessName).Distinct()
                    .Where((processName) => _allowedProcessesToClose.ContainsKey(processName))
                        .Select((processName) => _allowedProcessesToClose[processName]).ToArray();

            return currentProcesses.Length == 0 ? Array.Empty<string>() : currentProcesses;
        }
        private Task KillAllProcesses(Process[] processes)
        {
            Parallel.ForEach(processes, async (process) =>
            {
                process.Kill();
                await process.WaitForExitAsync();
            });
            return Task.CompletedTask;
        }
    }
}
