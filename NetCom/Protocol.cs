using System;
using System.Drawing;
using System.Net.Sockets;
using System.Text;


namespace NetCom
{
    public class Protocol
    {

        private Byte[] bytes;

        private readonly NetworkStream stream;

        public Protocol(NetworkStream stream)
        {
            this.stream = stream;
        }

        #region Send
        public void Send(Int32 value)
            => stream.Write(BitConverter.GetBytes(value));

        public void Send(UInt32 value)
            => stream.Write(BitConverter.GetBytes(value));

        public void Send(Boolean value)
            => stream.Write(BitConverter.GetBytes(value));

        public void Send(Byte value)
            => stream.WriteByte(value);

        public void Send<T>(T value)
            where T : Enum
            => stream.WriteByte((Byte)(Object)value);


        public void Send(Color value)
            => stream.Write(new Byte[] { value.R, value.G, value.B });

        public void Send(String value)
        {
            Send(value.Length);
            stream.Write(Encoding.ASCII.GetBytes(value));
        }
        #endregion
        #region Receive
        public Int32 ReceiveInt32()
        {
            bytes = new Byte[4];
            stream.Read(bytes);

            return BitConverter.ToInt32(bytes);
        }

        public UInt32 ReceiveUInt32()
        {
            bytes = new Byte[4];
            stream.Read(bytes);

            return BitConverter.ToUInt32(bytes);
        }

        public Boolean ReceiveBoolean()
        {
            bytes = new Byte[1];
            stream.Read(bytes);

            return BitConverter.ToBoolean(bytes);
        }

        public Byte ReceiveByte()
        {
            bytes = new Byte[1];
            stream.Read(bytes);

            return bytes[0];
        }

        public T ReceiveEnum<T>()
            where T : Enum
        {
            bytes = new Byte[1];
            stream.Read(bytes);

            return (T)(Object)bytes[0];
        }

        public String ReceiveString()
        {
            bytes = new Byte[ReceiveInt32()];
            stream.Read(bytes);

            return Encoding.ASCII.GetString(bytes);
        }

        public Color ReceiveColor()
        {
            bytes = new Byte[3];
            stream.Read(bytes);

            return Color.FromArgb(255, bytes[0], bytes[1], bytes[2]);
        }
        #endregion

    }
}
