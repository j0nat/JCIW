using System;

namespace Server
{
    static class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting JCIW Server...");
            // Start listening
            ServerManager serverManager = new ServerManager();
            serverManager.Start();

            Console.WriteLine("Server started.");

            while (true)
            {
                Console.ReadLine();
            }
        }
    }
}
