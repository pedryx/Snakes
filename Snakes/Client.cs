using NetCom;

using Snakes_Client.SnakeTypes;

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using System.Windows.Media;


namespace Snakes_Client
{
    public class Client
    {

        public static Int32 Port
        {
            get => 2227;
        }

        private readonly NetworkStream stream;
        private readonly Protocol protocol;
        private readonly Queue<Snake> snakesToAdd;

        private Byte id;

        public event SnakeEventHandler SnakeAdded;
        public event RemoveSnakeEventHandler SnakeRemoved;
        public event MapChangeEventHandler MapChanged;
        public event GameInitEventHandler GameStarted;
        public event EventHandler Update;
        public event MovementEventHandler OnSnakeMove;

        public Client(TcpClient client)
        {
            stream = client.GetStream();
            protocol = new Protocol(stream);
            snakesToAdd = new Queue<Snake>();
        }

        public IEnumerable<Snake> ReceiveInit(out Map map)
        {
            id = protocol.ReceiveByte();

            String file = protocol.ReceiveString();
            using (StreamWriter writter = new StreamWriter(file))
            {
                Int32 lineCount = protocol.ReceiveInt32();
                for (Int32 i = 0; i < lineCount; i++)
                    writter.Write(protocol.ReceiveString());
            }
            map = new Map(file);

            Snake[] snakes = new Snake[protocol.ReceiveInt32()];
            for (Int32 i = 0; i < snakes.Length; i++)
            {
                UInt32 id = protocol.ReceiveUInt32();
                Color color = Texture.ConvertColor(protocol.ReceiveColor());

                if (protocol.ReceiveBoolean())
                    snakes[i] = new AISnake(color, id);
                else
                    snakes[i] = new NetworkSnake(color, id);
            }

            return snakes;
        }

        public void StartReceiving()
        {
            Thread thread = new Thread(Receiving)
            {
                IsBackground = true,
            };
            thread.Start();
        }

        public void SendAddSnakeRequest(Snake snake)
        {
            snakesToAdd.Enqueue(snake);
            protocol.Send(MessageType.AddSnakeRequest);
            protocol.Send(snake is AISnake);
            protocol.Send(Texture.ConvertColor(snake.Color));
        }

        public void SendRemoveSnake(UInt32 id)
        {
            protocol.Send(MessageType.RemoveSnake);
            protocol.Send(id);
        }

        public void SendMapChange(Map map)
        {
            protocol.Send(MessageType.MapChange);
            protocol.Send(map.File);
            using StreamReader reader = new StreamReader(map.File);
            String[] content = reader.ReadToEnd().Split('\n');
            protocol.Send(content.Length);
            for (Int32 i = 0; i < content.Length; i++)
                protocol.Send(content[i]);
            protocol.Send(map.SpawnCount);
        }

        public void SendGameOver()
        {
            protocol.Send(MessageType.GameOver);
        }

        public void SendStartGame()
        {
            protocol.Send(MessageType.StartGame);
        }

        public void SendMovement(UInt32 id, Direction direction)
        {
            protocol.Send(MessageType.Movement);
            protocol.Send(id);
            protocol.Send(direction);
        }

        private void Receiving()
        {
            while(true)
            {
                if (!stream.DataAvailable)

                {
                    Thread.Sleep(20);
                    continue;
                }

                switch(protocol.ReceiveEnum<MessageType>())
                {
                    case MessageType.AddSnakeRequestDenied:
                        snakesToAdd.Dequeue();
                        #region Ugly long string
                        MessageBox.Show("Cannot add new snake, because maximum number of snakes for this map has beem reached!\nIf you dont have any player snake's you can still spectate.");
                        #endregion
                        break;
                    case MessageType.NewSnake:
                        if (protocol.ReceiveByte() == id)
                        {
                            Snake snake = snakesToAdd.Dequeue();
                            snake.Id = protocol.ReceiveUInt32();
                            SnakeAdded?.Invoke(this, new SnakeEventArgs(null, snake));
                        }
                        else
                        {
                            UInt32 id = protocol.ReceiveUInt32();
                            System.Drawing.Color color = protocol.ReceiveColor();
                            SnakeAdded?.Invoke(this, new SnakeEventArgs(
                                new SnakeInfo(protocol.ReceiveBoolean(), color, id), null));
                        }
                        break;
                    case MessageType.RemoveSnake:
                        SnakeRemoved?.Invoke(this, new RemoveSnakeEventArgs(protocol.ReceiveUInt32()));
                        break;
                    case MessageType.MapChange:
                        String file = protocol.ReceiveString();
                        String[] fileContent = new String[protocol.ReceiveInt32()];
                        for (Int32 i = 0; i < fileContent.Length; i++)
                            fileContent[i] = protocol.ReceiveString();
                        MapChanged?.Invoke(this, new MapChangeEventArgs(new Map(file)));
                        break;
                    case MessageType.StartGame:
                        GameStarted?.Invoke(this, new GameInitEventArgs(protocol.ReceiveInt32()));
                        break;
                    case MessageType.Update:
                        Update?.Invoke(this, new EventArgs());
                        break;
                    case MessageType.Movement:
                        OnSnakeMove?.Invoke(this,
                            new MovementEventArgs(protocol.ReceiveUInt32(), protocol.ReceiveEnum<Direction>()));
                        break;
                }
            }
        }

    }
}
