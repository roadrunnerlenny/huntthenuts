using System;

namespace SilverlightSnake
{
    public class SnakeEventArgs : EventArgs
    {
        public int Value { get; set; }

        public int Time { get; set; }

        public SnakeEventArgs(int value, int time) : base()
        {
            this.Value = value;
            this.Time = time;
        }
    }
}
