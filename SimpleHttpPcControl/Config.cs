using System.Text.Json.Serialization;

namespace SimpleHttpPcControl
{
    internal class Config
    {
        /// <summary>
        /// Information about this server.
        /// </summary>
        public ServerInfo Server { get; set; } = new();
        /// <summary>
        /// List of URLs to listen to.
        /// </summary>
        public string[] UrlToListen { get; set; } = Array.Empty<string>();
        /// <summary>
        /// All possible actions.
        /// </summary>
        public CommandAction[] Actions { get; set; } = Array.Empty<CommandAction>();
    }

    internal class ServerInfo
    {
        public string Name { get; set; } = string.Empty;
    }

    internal class CommandAction
    {
        /// <summary>
        /// Name for this action.
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Title for this action.
        /// </summary>
        public string Title { get; set; } = string.Empty;
        /// <summary>
        /// false (default): This action is not available.
        /// </summary>
        public bool Enabled { get; set; }
        /// <summary>
        /// Shell Command to execute.
        /// </summary>
        public string ShellCommand { get; set; } = string.Empty;
        /// <summary>
        /// Shell Command Parameters to use.
        /// </summary>
        public string? ShellCommandArguments { get; set; }

        public string GetPropertyValueByName(string name)
        {
            string Result = string.Empty;

            switch ((name ?? "").ToLower())
            {
                case "name": Result = Name.ToString().ToLower(); break;
                case "title": Result = Title ?? string.Empty; break;
                case "shellcommand": Result = ShellCommand ?? string.Empty; break;
                case "shellcommandarguments": Result = ShellCommandArguments ?? string.Empty; break;
            }
            return Result;
        }
    }
}
