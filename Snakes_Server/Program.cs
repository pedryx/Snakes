using System;

namespace Snakes_Server
{
    class Program
    {
        static void Main(String[] args)
        {
            Server server;
            if (args.Length > 0)
                server = new Server(args[0], Int32.Parse(args[1]), true);
            else
                server = new Server("Maps/Rocks.map", 4, false);
            server.Start();
        }
    }
}
