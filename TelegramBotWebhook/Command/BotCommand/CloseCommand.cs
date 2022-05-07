using System.Diagnostics;

namespace TelegramBotWebhook.Command.BotCommand
{
    internal class CloseCommand : BotCommand, ILongRunningCommand
    {
        static private Dictionary<string, string> allowedProcessesToClose = new Dictionary<string, string>
        {
            ["CiscoCollabHost"] = "Cisco Desktop",
            ["atmgr"] = "Cisco Web",
        };
        string? processToCloseName;

        public event Action? ExecuteIsOver;

        public CloseCommand() : base("close") { }

        public override object Clone() => new CloseCommand();
        public override async Task<ExecuteResult> Execute(string option)
        {
            if (option == String.Empty)
            {
                string[] processesToClose = FindProcessesToClose();

                if (processesToClose.Length == 0)
                {
                    return new ExecuteResult(ResultType.Text, "No process to close.");
                }
                else
                {
                    return new ExecuteResult(ResultType.Keyboard, "Select a process to close.", processesToClose);
                }
            }
            else if (option != String.Empty && processToCloseName is null)
            {
                if (allowedProcessesToClose.ContainsValue(option))
                {
                    processToCloseName = allowedProcessesToClose.Where((pair) => pair.Value == option)
                                                                    .Select((pair) => pair.Key).First();
                    return new ExecuteResult(ResultType.Keyboard, $"Do you really want to close the process {option}?", new string[] { "Yes", "No" });
                }
                else
                {
                    return new ExecuteResult(ResultType.Text, "Invalid process.");
                }
            }
            else
            {
                if (option.ToLower() == "yes")
                {
                    Process[] processes = Process.GetProcessesByName(processToCloseName);

                    if (processes.Length == 0)
                    {
                        ExecuteIsOver?.Invoke();
                        return new ExecuteResult(ResultType.Text, $"The process {allowedProcessesToClose[processToCloseName!]} has already closed.");
                    }
                    else
                    {
                        await KillAllProcesses(processes);
                        ExecuteIsOver?.Invoke();
                        return new ExecuteResult(ResultType.Text, $"The process {allowedProcessesToClose[processToCloseName!]} has closed.");
                    }
                } 
                else
                {
                    ExecuteIsOver?.Invoke();
                    return new ExecuteResult(ResultType.RemoveKeyboard);
                } 
            }
        }
        private string[] FindProcessesToClose()
        {
            var currentProcesses = Process.GetProcesses().AsParallel()
                .Select((process) => process.ProcessName).Distinct()
                    .Where((processName) => allowedProcessesToClose.ContainsKey(processName))
                        .Select((processName) => allowedProcessesToClose[processName]).ToArray();

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
