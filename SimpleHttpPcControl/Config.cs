using System.Text.Json.Serialization;

namespace SimpleHttpPcControl
{
    internal class Config
    {
        /// <summary>
        /// List of URLs to listen to.
        /// </summary>
        public string[]? UrlToListen { get; set; }
        /// <summary>
        /// All possible actions.
        /// </summary>
        public CommandAction[]? Actions { get; set; }
    }

    internal class CommandAction
    {
        /// <summary>
        /// Name for this action.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Common.WebServerActions Name { get; set; }
        /// <summary>
        /// Title for this action.
        /// </summary>
        public string? Title { get; set; }
        /// <summary>
        /// false (default): This action is not available.
        /// </summary>
        public bool Enabled { get; set; }
        /// <summary>
        /// Shell Command to execute.
        /// </summary>
        public string? ShellCommand { get; set; }
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
