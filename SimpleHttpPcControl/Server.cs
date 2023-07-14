using System.Diagnostics;
using System.Net;
using System.Text;

namespace SimpleHttpPcControl
{
    internal partial class Server
    {
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

            HttpListener listener = new HttpListener();
            foreach (string s in Common.Config.UrlToListen)
                listener.Prefixes.Add(s);

            listener.Start();
            Console.WriteLine("SimpleHttpPcControl is Listening...");
            try
            {
                while (true)
                {
                    HttpListenerContext context = listener.GetContext();
                    BuildResponse(context);
                }
            }
            catch (Exception)
            {
                listener.Stop();
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
            var Shell = Common.Config.Actions.FirstOrDefault(x => x.Name == RequestAction);

            switch (RequestAction)
            {
                case Common.WebServerActions.DoNothing:
                    BuildEmptyResponse(context);
                    break;

                case Common.WebServerActions.GetFavicon:
                    BuildFaviconResponse(context);
                    break;

                case Common.WebServerActions.Sleep:
                case Common.WebServerActions.Shutdown:
                    if (!string.IsNullOrWhiteSpace(Shell?.ShellCommand))
                        Process.Start(
                            Environment.ExpandEnvironmentVariables(Shell.ShellCommand), 
                            Shell.ShellCommandArguments ?? string.Empty);
                    break;

                case Common.WebServerActions.GetIndexPage:
                default:
                    BuildGetIndexPageResponse(context);
                    break;
            }
        }

        /// <summary>
        /// Send the favicon.ico file.
        /// </summary>
        /// <param name="context"></param>
        void BuildFaviconResponse(HttpListenerContext context)
        {
            byte[] buffer = File.ReadAllBytes(Common.GetCurrentExecutionFolder("data","favicon.ico"));
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
        Common.WebServerActions GetRequestAction(HttpListenerRequest request)
        {
            var Result = Common.WebServerActions.GetIndexPage;
            var Action = request.Headers["action"]?.ToLower();
            var ValidAction = request.HttpMethod == "POST"
                && request.Url?.AbsolutePath == "/";

            if (ValidAction && Action == "sleep")
                Result = Common.WebServerActions.Sleep;
            else if (ValidAction && Action == "shutdown")
                Result = Common.WebServerActions.Shutdown;
            else if (request.HttpMethod == "GET"
                && request.Url?.AbsolutePath?.Equals("/favicon.ico", StringComparison.InvariantCultureIgnoreCase) == true)
                Result = Common.WebServerActions.GetFavicon;
            else if (request.Url?.AbsolutePath != "/")
                Result = Common.WebServerActions.DoNothing;
            return Result;
        }

    }
}
