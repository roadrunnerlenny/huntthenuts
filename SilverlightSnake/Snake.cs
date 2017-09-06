using System;
using System.Collections.Generic;

namespace SilverlightSnake
{
    public class Snake
    {
        public int SnakePosX { get; set; }

        public int SnakePosY { get; set; }

        public Level CurLevel { get; set; }

        public SnakeUtils.Directions NewDirection { get; set; }

        internal SnakeUtils.Directions OldDirection { get; set; }

        protected int IsShrinking = 0;

        protected LinkedList<Tail> _tailList = new LinkedList<Tail>();
        internal LinkedList<Tail> TailList
        {
            get { return _tailList; }
        }

        public Snake(Level curLevel)
        {
            CurLevel = curLevel;
            SnakePosX = curLevel.SnakeStartPosX;
            SnakePosY = curLevel.SnakeStartPosY;

            curLevel.ShrinkingEvent += new Level.ShrinkingEventHandler(OnShrinking);

        }

        /* Position in Grid only; */
        public void Move()
        {
            switch (OldDirection) {
                case SnakeUtils.Directions.Up: SnakePosY -= 1;
                    break;
                case SnakeUtils.Directions.Right: SnakePosX += 1;
                    break;
                case SnakeUtils.Directions.Down: SnakePosY += 1;
                    break;
                case SnakeUtils.Directions.Left: SnakePosX -= 1;
                    break;
            }

            CollisionDetectTail();

            foreach (Tail tail in TailList)
                tail.MoveTail();

            if (IsShrinking > 0)
                Shrink();

            CollisionDetect();

            LinkedListNode<Tail> firstTailNode = TailList.First;
            if (firstTailNode != null)
            {
                firstTailNode.Value.NewDirection = firstTailNode.Value.OldDirection;
                firstTailNode.Value.OldDirection = OldDirection; //curDirection
                
                LinkedListNode<Tail> nextTail = firstTailNode;
                while ((nextTail = nextTail.Next) != null)
                {
                    nextTail.Value.NewDirection = nextTail.Value.OldDirection;
                    nextTail.Value.OldDirection = nextTail.Previous.Value.NewDirection;
                }
            }

        }

        public bool ChangeDirection()
        {
            switch (OldDirection)
            {
                case SnakeUtils.Directions.Up: 
                    if (NewDirection != SnakeUtils.Directions.Down 
                    && NewDirection != SnakeUtils.Directions.Up) 
                    OldDirection = NewDirection; 
                    return true;
                case SnakeUtils.Directions.Down: 
                    if (NewDirection != SnakeUtils.Directions.Up && NewDirection != SnakeUtils.Directions.Down) 
                        OldDirection = NewDirection; 
                        return true;
                case SnakeUtils.Directions.Left: 
                    if (NewDirection != SnakeUtils.Directions.Right && NewDirection != SnakeUtils.Directions.Left) 
                        OldDirection = NewDirection; 
                        return true;
                case SnakeUtils.Directions.Right: 
                    if (NewDirection != SnakeUtils.Directions.Left && NewDirection != SnakeUtils.Directions.Right) 
                    OldDirection = NewDirection; 
                    return true;
                case SnakeUtils.Directions.Stop: OldDirection = NewDirection; return true;
            }
            return false;
        }

        public bool ChangeDirection(SnakeUtils.Directions newDirection) {
            NewDirection = newDirection;
            return ChangeDirection();
        }


        protected SnakeUtils.Collision CollisionImminent()
        {
            switch (OldDirection)
            {
                case SnakeUtils.Directions.Up:
                    if (SnakePosY - 1 < 0) return SnakeUtils.Collision.Wall;
                    foreach (Tail tail in TailList)
                        if (tail.TailPosX == SnakePosX && tail.TailPosY == SnakePosY - 1) return SnakeUtils.Collision.Tail;
                    if (CurLevel.Apples[SnakePosX, SnakePosY-1] == true) return SnakeUtils.Collision.Apple;
                    if (CurLevel.Objects[SnakePosX, SnakePosY-1]) return SnakeUtils.Collision.Object;
                    break;
                case SnakeUtils.Directions.Left:
                    if (SnakePosX - 1 < 0) return SnakeUtils.Collision.Wall;
                    foreach (Tail tail in TailList)
                        if (tail.TailPosX == SnakePosX - 1 && tail.TailPosY == SnakePosY) return SnakeUtils.Collision.Tail;
                    if (CurLevel.Apples[SnakePosX-1, SnakePosY] == true) return SnakeUtils.Collision.Apple;
                    if (CurLevel.Objects[SnakePosX-1, SnakePosY]) return SnakeUtils.Collision.Object;
                    break;
                case SnakeUtils.Directions.Right:
                    if (SnakePosX + 1 >= CurLevel.LevelWidth) return SnakeUtils.Collision.Wall;
                    foreach (Tail tail in TailList)
                        if (tail.TailPosX == SnakePosX + 1 && tail.TailPosY == SnakePosY) return SnakeUtils.Collision.Tail;
                    if (CurLevel.Apples[SnakePosX+1, SnakePosY] == true) return SnakeUtils.Collision.Apple;
                    if (CurLevel.Objects[SnakePosX+1, SnakePosY]) return SnakeUtils.Collision.Object;
                    break;
                case SnakeUtils.Directions.Down:
                    if (SnakePosY + 1 >= CurLevel.LevelHeight) return SnakeUtils.Collision.Wall;
                    foreach (Tail tail in TailList)
                        if (tail.TailPosX == SnakePosX && tail.TailPosY == SnakePosY + 1) return SnakeUtils.Collision.Tail;
                    if (CurLevel.Apples[SnakePosX, SnakePosY+1] == true) return SnakeUtils.Collision.Apple;
                    if (CurLevel.Objects[SnakePosX, SnakePosY+1]) return SnakeUtils.Collision.Object;
                    break;
            }

            return SnakeUtils.Collision.Nothing;

        }

        protected void CollisionDetect()
        {
            if (SnakePosX < 0 || SnakePosX >= CurLevel.LevelWidth || SnakePosY < 0 || SnakePosY >= CurLevel.LevelHeight) throw new CollisionWallHitException();

            if (CurLevel.Objects[SnakePosX, SnakePosY]) throw new CollisionHitObjectException();
            
            if (CurLevel.Apples[SnakePosX,SnakePosY])
            {
                CurLevel.EatApple(SnakePosX, SnakePosY);

                foreach(Apple apple in CurLevel.AppleObjects)
                    if (apple.ApplePosX == SnakePosX && apple.ApplePosY == SnakePosY)
                        for (int i=0;i<apple.TailGrow;i++)
                            AddTail(CreateTail());

                bool[,] forbidden = new bool[CurLevel.LevelWidth, CurLevel.LevelHeight];
                forbidden[SnakePosX, SnakePosY] = true;
                try
                {
                    foreach (Tail tail in TailList)
                        forbidden[tail.TailPosX, tail.TailPosY] = true;
                } catch (IndexOutOfRangeException e)
                { //this Exception Occurs only if the TailGrow is set for an Apple
                  // and the TailGrow is greater than 1; if the new Tail exceed the borders of the game fields,
                  // an IndexOutOfRangeException is thrown
                    ; 
                }
                foreach (Apple apple in CurLevel.AppleObjects)
                {
                    if (apple.ApplePosX == SnakePosX && apple.ApplePosY == SnakePosY)
                        CurLevel.GenerateApple(forbidden, apple);
                    else if (apple.IsSpecial && !apple.IsSet)
                        CurLevel.GenerateApple(forbidden, apple);
                }
            }

        }

        private void CollisionDetectTail() { 
            foreach (Tail tail in TailList)
                if (tail.TailPosX == SnakePosX && tail.TailPosY == SnakePosY) throw new CollisionHitItselfException();
        }

        protected virtual void AddTail(Tail tail)
        {
            TailList.AddLast(tail);
        }

        protected virtual Tail CreateTail()
        {
            if (TailList.Last == null) return new Tail(NewDirection, SnakePosX, SnakePosY, CurLevel);
            else return new Tail(TailList.Last.Value.OldDirection, TailList.Last.Value.TailPosX, TailList.Last.Value.TailPosY, CurLevel);
        }

        public virtual void OnShrinking(object sender, SnakeEventArgs e)
        {
            IsShrinking = e.Value;
        }

        protected virtual void Shrink()
        {
            for (int count = 0; count < IsShrinking; count++)
            {
                if (TailList.Last != null)
                {
                    LinkedListNode<Tail> lastNode = TailList.Last;
                    Tail last = (Tail)lastNode.Value;
                    TailList.Remove(last);
                }
            }

            IsShrinking = 0;
        }

        public virtual void SetToStart()
        {
            TailList.Clear();
            CurLevel.RemoveApples();
        }
    }
}
