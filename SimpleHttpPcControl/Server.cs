using System.Diagnostics;
using System.Net;
using System.Text;

namespace SimpleHttpPcControl
{
    internal partial class Server
    {
        const string StartupMessage = "SimpleHttpPcControl is Listening...";

        readonly string[] RecognizedFavIcons = new string[] {
                "android-chrome-192x192.png",
                "android-chrome-512x512.png",
                "apple-touch-icon.png",
                "favicon-16x16.png",
                "favicon-32x32.png",
                "favicon.ico"
            };

        /// <summary>
        /// Configure and start listening in the provided URL's.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public void Listen()
        {
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                return;
            }

            // Url Must be Reserved: netsh http add urlacl url=http://127.0.0.1:8080/ user=Everyone listen=yes
            // Firewall: netsh firewall add portopening TCP 8080 SimpleHttpPcControl enable ALL
            if (Common.Config?.UrlToListen?.Any() != true)
                throw new ArgumentException("prefixes");

            HttpListener Listener = new HttpListener();

            Common.RetryCommand(() =>
            {
                foreach (string s in Common.Config.UrlToListen)
                    Listener.Prefixes.Add(s);
                Listener.Start();
            }, () => Listener = new HttpListener());

            Console.WriteLine(StartupMessage);
            Common.Log(StartupMessage);

            try
            {
                while (true)
                {
                    HttpListenerContext context = Listener.GetContext();
                    BuildResponse(context);
                }
            }
            catch (Exception)
            {
                Listener.Stop();
                throw;
            }

        }

        /// <summary>
        /// Builds and send the response.
        /// </summary>
        /// <param name="context"></param>
        void BuildResponse(HttpListenerContext context)
        {
            var RequestAction = GetRequestAction(context.Request);
            var Shell = Common.Config.Actions!.FirstOrDefault(x =>
                x.Name.Equals(RequestAction, StringComparison.InvariantCultureIgnoreCase));

            switch (RequestAction)
            {
                case "":
                    BuildGetIndexPageResponse(context);
                    break;

                case "getfavicon":
                    BuildFaviconResponse(context);
                    break;

                default:
                    if (!string.IsNullOrWhiteSpace(RequestAction)
                        && !string.IsNullOrWhiteSpace(Shell?.ShellCommand))
                        Process.Start(
                            Environment.ExpandEnvironmentVariables(Shell.ShellCommand),
                            Shell.ShellCommandArguments ?? string.Empty);
                    break;
            }
        }

        /// <summary>
        /// Send the favicon.ico file.
        /// </summary>
        /// <param name="context"></param>
        void BuildFaviconResponse(HttpListenerContext context)
        {
            var favIconName = RecognizedFavIcons.FirstOrDefault(i =>
                context.Request.Url?.AbsolutePath?.Equals($"/{i}",
                    StringComparison.InvariantCultureIgnoreCase) == true)
                ?? "favicon.ico";

            byte[] buffer = File.ReadAllBytes(Common.GetCurrentExecutionFolder("data", favIconName));
            context.Response.ContentLength64 = buffer.Length;
            context.Response.ContentType = "image/x-icon";
            Stream output = context.Response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }

        /// <summary>
        /// Send an empty response.
        /// </summary>
        /// <param name="context"></param>
        void BuildEmptyResponse(HttpListenerContext context)
        {
            context.Response.ContentLength64 = 0;
            context.Response.StatusCode = (int)HttpStatusCode.NoContent;
            context.Response.Close();
        }

        /// <summary>
        /// Build and send the home page.
        /// </summary>
        /// <param name="context"></param>
        void BuildGetIndexPageResponse(HttpListenerContext context)
        {
            string responseString = Page.GetIndexPageHtml();
            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            context.Response.ContentLength64 = buffer.Length;
            Stream output = context.Response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }

        /// <summary>
        /// Determines request and returns, the name, for what the user wants.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        string GetRequestAction(HttpListenerRequest request)
        {
            var Result = string.Empty;
            var Action = (request.Headers["action"] ?? string.Empty).ToLower();
            var ValidAction = request.HttpMethod == "POST"
                && request.Url?.AbsolutePath == "/";
            var Actions = Common.Config.Actions;


            if (ValidAction
                && Actions.Any(a =>
                    (a.Name ?? string.Empty).Equals(Action, StringComparison.InvariantCultureIgnoreCase)) == true)
                Result = Action;

            else if (request.HttpMethod == "GET"
                && RecognizedFavIcons.Any(i => request.Url?.AbsolutePath?.Equals($"/{i}", StringComparison.InvariantCultureIgnoreCase) == true))
                Result = "getfavicon";

            return Result;
        }

    }
}
