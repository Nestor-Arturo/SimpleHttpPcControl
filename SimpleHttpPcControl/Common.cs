using System.Reflection;
using System.Text.Json;

namespace SimpleHttpPcControl
{
    internal class Common
    {
        /// <summary>
        /// Enumeration for all possible responses.
        /// </summary>
        internal enum WebServerActions
        {
            DoNothing,
            GetIndexPage,
            GetFavicon,
            Sleep,
            Shutdown
        }

        static Config? _Config = null;
        /// <summary>
        /// The application configuration.
        /// </summary>
        static internal Config Config
        { get
            {
                if (_Config == null)
                {
                    var JSONFile = GetCurrentExecutionFolder("data", "config.json");
                    string JSONConfigText = File.ReadAllText(JSONFile) ?? string.Empty;
                    _Config = JsonSerializer.Deserialize<Config>(JSONConfigText,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                    if (_Config?.Actions?.Any() == true)
                        _Config.Actions = _Config.Actions.Where(a => a.Enabled).ToArray();
                }
                if (_Config == null)
                    throw new Exception("Unable to read configuration file.");
                return _Config;
            }
        }

        /// <summary>
        /// Returns the current execution path. Optional: add child segments to the path.
        /// </summary>
        /// <param name="addChildPath"></param>
        /// <returns></returns>
       static internal string GetCurrentExecutionFolder(params string[] addChildPath)
        {
            var Result = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (addChildPath != null && !string.IsNullOrWhiteSpace(Result))
                Result = Path.Combine(Result, Path.Combine(addChildPath));
            return Result ?? string.Empty;
        }
    }
}
