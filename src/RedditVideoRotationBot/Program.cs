namespace RedditVideoRotationBot
{
    public class Program
    {
        static void Main(string[] args)
        {
            var worker = new RedditHelper();
            worker.ReadMessages();
        }
    }
}
