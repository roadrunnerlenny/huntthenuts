using System;
using System.Windows;
using System.Windows.Controls;


namespace SilverlightSnake
{
    public class SLLevelObject : LevelObject
    {
        public double OffsetY { get; set; }

        private Point _pos = new Point();
        public Point Pos
        {
            get { return _pos; }
            set { _pos = value; }
        }

        public Image ObjectImage { get; set; }
    }
}
