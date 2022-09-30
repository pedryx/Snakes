using System;

namespace Snakes_Client
{

    public delegate void RemoveSnakeEventHandler(Object sender, RemoveSnakeEventArgs e);

    public class RemoveSnakeEventArgs
    {

        public UInt32 Id { get; private set; }

        public RemoveSnakeEventArgs(UInt32 id)
        {
            Id = id;
        }

    }
}
