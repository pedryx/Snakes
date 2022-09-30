using System;

namespace NetCom
{
    /// <summary>
    /// Repredents a type of a message thats is being send over the network.
    /// Ussualy appear at the start of the message.
    /// </summary>
    public enum MessageType : Byte
    {
        AddSnakeRequest,
        AddSnakeRequestDenied,
        NewSnake,
        RemoveSnake,
        MapChange,
        StartGame,
        Update,
        GameOver,
        Movement,
    }
}
