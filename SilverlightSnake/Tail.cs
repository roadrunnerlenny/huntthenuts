using System;
using System.Collections.Generic;

namespace SilverlightSnake
{
    public class Tail
    {
        public int TailPosX { get; set; }

        public int TailPosY { get; set; }

        public Level CurLevel { get; set; }

        public bool IsGrowing { get; set; }

        protected SnakeUtils.Directions _newDirection = SnakeUtils.Directions.Stop;
        public SnakeUtils.Directions NewDirection
        {
            get { return _newDirection; }
            set { _newDirection = value; }
        }

        protected SnakeUtils.Directions _oldDirection = SnakeUtils.Directions.Stop;
        internal SnakeUtils.Directions OldDirection
        {
            get { return _oldDirection; }
            set { _oldDirection = value; }
        }

        public Tail(SnakeUtils.Directions direction, int tailPosX, int tailPosY, Level curLevel)
        {
            CurLevel = curLevel;
            _oldDirection = direction;
            TailPosX = tailPosX;
            TailPosY = tailPosY;

            IsGrowing = true;

            switch (direction)
            {
                case SnakeUtils.Directions.Up: TailPosY += 1;
                    break;
                case SnakeUtils.Directions.Right: TailPosX -= 1;
                    break;
                case SnakeUtils.Directions.Down: TailPosY -= 1;
                    break;
                case SnakeUtils.Directions.Left: TailPosX += 1;
                    break;
            }
        }

        public void MoveTail()
        {
            switch (_oldDirection)
            {
                case SnakeUtils.Directions.Up: TailPosY -= 1;
                    break;
                case SnakeUtils.Directions.Right: TailPosX += 1;
                    break;
                case SnakeUtils.Directions.Down: TailPosY += 1;
                    break;
                case SnakeUtils.Directions.Left: TailPosX -= 1;
                    break;
            }

            IsGrowing = false;
        }

        public virtual void SetToStart()
        {
            ;
        }
    }
}
