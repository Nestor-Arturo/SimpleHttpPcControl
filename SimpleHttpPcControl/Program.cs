namespace SimpleHttpPcControl
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var ServerObject = new Server();
            ServerObject.Listen();
        }
    }
}