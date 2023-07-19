using System.Reflection;
using System.Text.Json;

namespace SimpleHttpPcControl
{
    internal class Common
    {
        static Config? _Config = null;
        /// <summary>
        /// The application configuration.
        /// </summary>
        static internal Config Config
        {
            get
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
                if (_Config?.Actions?.Any() != true
                    || _Config?.UrlToListen?.Any() != true)
                    throw new Exception("Unable to read configuration file or configuration file is incomplete.");
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

        /// <summary>
        /// Retries an Action 12 times (default). Waits 5 seconds (default) between tries. true: action finished without exception thrown.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="maxTries"></param>
        /// <param name="retryWaitTimeMs"></param>
        /// <returns></returns>
        static internal bool RetryCommand(Action action, Action? onException = null, bool reThrowLastException = true, int maxTries = 12, int retryWaitTimeMs = 5000)
        {
            if (action == null) return false;
            Exception? LastException = null;
            for (int i = 0; i < maxTries; i++)
            {
                try
                {
                    action();
                    return true;
                }
                catch (Exception ex)
                {
                    LastException = ex;
                    Log($"Retriying action ({i + 1}/{maxTries}). Waiting {retryWaitTimeMs}ms. Reason: {ex.Message}");
                    if (onException != null)
                        onException();
                    if ((maxTries - i) == 1) break;
                    Task.Delay(retryWaitTimeMs).Wait();
                }
            }

            if (reThrowLastException && LastException != null) 
                throw LastException;

            return false;
        }

        #region LOG

        /// <summary>
        /// Adds an extension to the log file.
        /// </summary>
        /// <param name="ex"></param>
        static internal void Log(Exception ex)
        {
            Log($"{ex.Message}\t{ex.StackTrace}");
        }

        /// <summary>
        /// Adds text to the log file.
        /// </summary>
        /// <param name="message"></param>
        static internal void Log(string message)
        {
            var LogFolder = GetCurrentExecutionFolder("log");
            if (!Directory.Exists(LogFolder))
                Directory.CreateDirectory(LogFolder);
            var LogFile = Path.Combine(LogFolder, $"{DateTime.Now.ToString("yyyy-MM-dd")}.txt");

            File.AppendAllText(LogFile,
                $"{DateTime.Now.ToString("HH:mm:ss")}\t{message}\r\n");

            _ = CleanOldLogFilesAsync();
        }

        static private async Task CleanOldLogFilesAsync()
        {
            var LogFolder = GetCurrentExecutionFolder("log");
            if (!Directory.Exists(LogFolder)) return;

            var DeletableLogs = (new DirectoryInfo(LogFolder)).GetFiles()
                .Where(f => (DateTime.Now - f.LastWriteTime).TotalDays > 10)
                .ToArray();

            foreach (var LogFile in DeletableLogs)
            {
                try
                {
                    await Task.Run(() => LogFile.Delete());
                }
                catch { }
            }
        }

        #endregion
    }
}
