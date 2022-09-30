using System;
using System.Net.Sockets;

namespace Snakes_Server
{
    class ClientArgs
    {

        public NetworkStream Stream { get; private set; }

        public Byte Id { get; private set; }

        public ClientArgs(NetworkStream stream, Byte id)
        {
            Stream = stream;
            Id = id;
        }

    }
}
