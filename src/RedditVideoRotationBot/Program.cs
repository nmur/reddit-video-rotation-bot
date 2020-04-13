using System;

namespace RedditVideoRotationBot
{
    public class Program
    {
        static void Main(string[] args)
        {
            var counter = 0;
            while (counter < int.MaxValue)
            {
                counter++;
                Console.WriteLine($"Counter: {counter}!!!");
                System.Threading.Tasks.Task.Delay(5000).Wait();
            }
        }
    }
}
