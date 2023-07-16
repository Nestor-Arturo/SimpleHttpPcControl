namespace SimpleHttpPcControl
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var ServerObject = new Server();
                ServerObject.Listen();
            }
            catch (Exception ex)
            {
                Common.Log(ex);
                throw;
            }
        }
    }
}