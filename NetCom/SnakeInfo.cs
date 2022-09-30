using System;
using System.Drawing;

namespace NetCom
{
    public class SnakeInfo
    {

        public Boolean AI { get; set; }

        public Color Color { get; set; }

        public UInt32 Id { get; set; }

        public SnakeInfo(Boolean ai, Color color, UInt32 id)
        {
            AI = ai;
            Color = color;
            Id = id;
        }

    }
}
