using System;
using System.Windows;
using System.Windows.Controls;

namespace SilverlightSnake
{
    public class SLApple : Apple
    {
        public Image AppleImage { get; set; }

        private Point _pos = new Point();
        public Point Pos
        {
            get { return _pos; }
            set { _pos = value; }
        }

        public double AppleOffsetY { get; set; }
    }
}
