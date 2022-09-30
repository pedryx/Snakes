using NetCom;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace Snakes_Server
{
    class Server
    {

        private const Int32 port = 2227;

        private readonly Object clientsLock = new Object();
        private readonly TcpListener listener;
        private readonly Dictionary<UInt32, SnakeInfo> snakes;
        private readonly Dictionary<Byte, TcpClient> clients;
        private readonly Boolean args;

        private Byte lastId = 0;
        private UInt32 lastSnakeId = 0;

        private Int32 maxSnakeCount;
        private String map;
        private Timer timer;
        private Boolean exit;

        public Server(String map, Int32 maxSnakeCount, Boolean args)
        {
            this.map = map;
            this.maxSnakeCount = maxSnakeCount;
            this.args = args;

            listener = new TcpListener(IPAddress.Any, port);
            clients = new Dictionary<Byte, TcpClient>();
            snakes = new Dictionary<UInt32, SnakeInfo>();

            if (args)
                snakes.Add(lastSnakeId, new SnakeInfo(false, Color.FromArgb(255, 255, 0, 0), lastSnakeId++));
        }

        public void Start()
        {
            listener.Start();
            Console.WriteLine($"Server started! - port: {port}");
            while (!exit)
            {
                if (!listener.Pending())
                {
                    Thread.Sleep(20);
                    continue;
                }

                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine($"{client.Client.LocalEndPoint} - {lastId}");
                if (clients.Count != 0 || !args)
                {
                    lock (client)
                        SendInit(client.GetStream(), lastId);
                }
                lock (clientsLock)
                    clients.Add(lastId, client);

                (new Thread(Receiving)
                {
                    IsBackground = true,
                }).Start(new ClientArgs(client.GetStream(), lastId++));
            }
        }

        private void Receiving(Object args)
        {
            ClientArgs clientArgs = (ClientArgs)args;
            Protocol protocol = new Protocol(clientArgs.Stream);

            while (!exit)
            {
                if (!clientArgs.Stream.DataAvailable)
                {
                    Thread.Sleep(20);
                    continue;
                }

                switch (protocol.ReceiveEnum<MessageType>())
                {
                    case MessageType.AddSnakeRequest:
                        if (snakes.Count >= maxSnakeCount)
                        {
                            lock (clients[clientArgs.Id])
                                protocol.Send(MessageType.AddSnakeRequestDenied);
                        }
                        else
                            SendNewSnake(clientArgs.Id,
                                new SnakeInfo(protocol.ReceiveBoolean(), protocol.ReceiveColor(), lastSnakeId++));
                        break;
                    case MessageType.RemoveSnake:
                        SendRemoveSnake(protocol.ReceiveUInt32());
                        break;
                    case MessageType.MapChange:
                        map = protocol.ReceiveString();
                        String[] fileContent = new String[protocol.ReceiveInt32()];
                        for (Int32 i = 0; i < fileContent.Length; i++)
                            fileContent[i] = protocol.ReceiveString();
                        maxSnakeCount = protocol.ReceiveInt32();
                        SendMapChange(map, fileContent);
                        break;
                    case MessageType.StartGame:
                        SendStartGame();
                        break;
                    case MessageType.GameOver:
                        exit = true;
                        timer.Dispose();
                        break;
                    case MessageType.Movement:
                        SendMovement(protocol.ReceiveUInt32(), protocol.ReceiveEnum<Direction>());
                        break;
                }
            }
        }

        private void SendMovement(UInt32 id, Direction direction)
        {
            SendToAll((client, protocol) =>
            {
                protocol.Send(MessageType.Movement);
                protocol.Send(id);
                protocol.Send(direction);
            });
        }

        private void SendStartGame()
        {
            Int32 seed = (Int32)DateTime.Now.Ticks;
            SendToAll((client, protocol) =>
            {
                protocol.Send(MessageType.StartGame);
                protocol.Send(seed);
            });
            timer = new Timer(Timer_Callback, null, 500, 500);
        }

        private void SendMapChange(String fileName, String[] fileContent)
        {
            SendToAll((client, protocol) =>
            {
                protocol.Send(MessageType.MapChange);
                protocol.Send(fileName);
                protocol.Send(fileContent.Length);
                for (Int32 i = 0; i < fileContent.Length; i++)
                    protocol.Send(fileContent[i]);
            });
        }

        private void SendRemoveSnake(UInt32 id)
        {
            snakes.Remove(id);
            SendToAll((client, protocol) =>
            {
                protocol.Send(MessageType.RemoveSnake);
                protocol.Send(id);
            });
        }

        private void SendNewSnake(Byte id, SnakeInfo snake)
        {
            snakes.Add(snake.Id, snake);
            SendToAll((client, protocol) =>
            {
                protocol.Send(MessageType.NewSnake);
                protocol.Send(id);
                protocol.Send(snake.Id);

                if (clients[id] == client)
                    return;

                protocol.Send(snake.Color);
                protocol.Send(snake.AI);
            });
        }

        private void SendToAll(Action<TcpClient, Protocol> action)
        {
            foreach (var client in clients.Values)
            {
                lock (client)
                    action.Invoke(client, new Protocol(client.GetStream()));
            }
        }

        private void SendInit(NetworkStream stream, Byte id)
        {
            Protocol protocol = new Protocol(stream);
            String[] fileContent;
            using (StreamReader reader = new StreamReader(map))
                fileContent = reader.ReadToEnd().Split('\n');

            // id
            protocol.Send(id);

            // map file
            protocol.Send(map);
            protocol.Send(fileContent.Length);
            foreach (var line in fileContent)
                protocol.Send(line);

            // snake list
            protocol.Send(snakes.Count);
            foreach (var snake in snakes.Values)
            {
                protocol.Send(snake.Id);
                protocol.Send(snake.Color);
                protocol.Send(snake.AI);
            }
        }

        private void Timer_Callback(Object state)
        {
            foreach (var client in clients.Values)
            {
                lock (client)
                {
                    Protocol protocol = new Protocol(client.GetStream());
                    protocol.Send(MessageType.Update);
                }
            }
        }

    }
}
