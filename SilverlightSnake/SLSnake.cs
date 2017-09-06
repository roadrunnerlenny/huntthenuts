using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;


namespace SilverlightSnake
{
    public class SLSnake : Snake
    {
        public Image SnakeImage { get; set; }

        private Image[,] _bufferedImage;

        public Image[,] BufferedImage
        {
            get { return _bufferedImage; }
            set { _bufferedImage = value; }
        }

        private Image[,] _bufferedCollisionImage;

        public Image[,] BufferedCollisionImage
        {
            get { return _bufferedCollisionImage; }
            set { _bufferedCollisionImage = value; }
        }

        private Dictionary<string, Image[]> _animationsDict = new Dictionary<string, Image[]>();

        public Dictionary<string, Image[]> AnimationsDict
        {
            get { return _animationsDict; }
            set { _animationsDict = value; }
        }

        public new SLLevel CurLevel { get; set; }

        private Point _pos;
        public Point Pos
        {
            get { return _pos; }
            set { _pos = value; }
        }

        private byte _tileCount = 0;
        private bool _isNewTurn = true;

        private byte _winAnimCount = 0;

        private SnakeUtils.Collision _imminentCollision = SnakeUtils.Collision.Nothing;

        public SnakeUtils.Collision ImminentCollision
        {
            get { return _imminentCollision; }
            set { _imminentCollision = value; }
        }

        private List<SnakeUtils.Directions> _keyBuffer = new List<SnakeUtils.Directions>();
        private bool HasNewDirectionAssigned { get; set; }

        public bool Animated { get; set; }

        public SLSnake(SLLevel curLevel)
            : base(curLevel)
        {
            CurLevel = curLevel;
      
            Pos = new Point(CurLevel.SnakeStartPosX * CurLevel.TileSize, CurLevel.SnakeStartPosY * CurLevel.TileSize);
            
            //Check if the Snake is animated and set necessary animation initialization
            Animated = CurLevel.AnimatedSnake;

            if (Animated)
            {

                _initSnakeAnimation();
                _initCollisionAnimation();
                SnakeUtils.InitAnimationInDict(AnimationsDict, "win", CurLevel.Animation.FilenameWin, CurLevel.Animation.WinFrames, CurLevel.LevelCanvas);
            } else
            {
                SnakeImage = SnakeUtils.LoadImage(CurLevel.FilenameHead);
                CurLevel.LevelCanvas.Children.Add(SnakeImage);
            }

            SnakeImage.SetValue(Canvas.LeftProperty, Pos.X);
            SnakeImage.SetValue(Canvas.TopProperty, Pos.Y + CurLevel.SnakeOffsetY);
            
        }

        public void _initSnakeAnimation() {
            _bufferedImage = new Image[CurLevel.Animation.AnimationFrames, 4];

            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < CurLevel.Animation.AnimationFrames; x++)
                {
                    _bufferedImage[x, y] = SnakeUtils.LoadImage(CurLevel.FilenameHead + "_" + (x + 1) + "_" + (y + 1));
                    _bufferedImage[x, y].Visibility = Visibility.Collapsed;
                    CurLevel.LevelCanvas.Children.Add(_bufferedImage[x, y]);
                }
            }

            /* Start with first animation frame */
            SnakeImage = BufferedImage[0, 0];
            SnakeImage.Visibility = Visibility.Visible;
        }

        public void _initCollisionAnimation()
        {
            _bufferedCollisionImage = new Image[CurLevel.Animation.CollisionFrames, 4];

            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < CurLevel.Animation.CollisionFrames; x++)
                {
                    _bufferedCollisionImage[x, y] = SnakeUtils.LoadImage(CurLevel.Animation.FilenameCollision + "_" + (x + 1) + "_" + (y + 1));
                    _bufferedCollisionImage[x, y].Visibility = Visibility.Collapsed;
                    CurLevel.LevelCanvas.Children.Add(_bufferedCollisionImage[x, y]);
                }
            }
        }


        public void MoveSL()
        {

            if (NewDirection == SnakeUtils.Directions.Stop)
                return;

            if (_isNewTurn == true)
            {
                base.ChangeDirection();
                //if (!directionChanged && _keyBuffer.Count > 0)
                //    _keyBuffer.Clear();
                _UpdateKeyBuffer();
                _isNewTurn = false;

                ImminentCollision = base.CollisionImminent();
            }


            if (ImminentCollision != SnakeUtils.Collision.Wall && ImminentCollision != SnakeUtils.Collision.Object && ImminentCollision != SnakeUtils.Collision.Tail)
            {
                SnakeUtils.MoveSLPosition(OldDirection, ref _pos, 1.0);
                foreach (SLTail tail in TailList)
                    tail.SLMoveTail(TailList);
            }


            _tileCount++;

            if (_tileCount >= CurLevel.TileSize)
            {
                base.Move();
                SnakeUtils.SetZIndex(CurLevel, this, TailList);

                _isNewTurn = true;
                _tileCount = 0;
            }

            if (Animated)
            {
                int frameNo = (_tileCount / CurLevel.Animation.AnimationDelay) % CurLevel.Animation.AnimationFrames;
                int frameColl = (_tileCount / CurLevel.Animation.CollisionDelay) % CurLevel.Animation.CollisionFrames;
                SnakeImage.Visibility = Visibility.Collapsed;
                if (ImminentCollision != SnakeUtils.Collision.Wall && ImminentCollision != SnakeUtils.Collision.Object && ImminentCollision != SnakeUtils.Collision.Tail)
                {
                    if (!CurLevel.IsControlInversed)
                        SnakeImage = BufferedImage[frameNo, ((int)this.OldDirection - 1)];
                    else
                        switch (this.OldDirection)
                        {
                            case SnakeUtils.Directions.Up: SnakeImage = BufferedImage[frameNo, 2]; break;
                            case SnakeUtils.Directions.Right: SnakeImage = BufferedImage[frameNo, 3]; break;
                            case SnakeUtils.Directions.Down: SnakeImage = BufferedImage[frameNo, 0]; break;
                            case SnakeUtils.Directions.Left: SnakeImage = BufferedImage[frameNo, 1]; break;
                        }
                        

                    foreach (SLTail tail in TailList)
                            tail.Animate(_tileCount);
                    
                } else
                {
                    //Add here Collision Animation, e.g. when hitting an object
                    SnakeImage = BufferedCollisionImage[frameColl, ((int)this.OldDirection - 1)];

                }
                SnakeImage.Visibility = Visibility.Visible;
            } else
            {
                if (ImminentCollision == SnakeUtils.Collision.Wall || ImminentCollision == SnakeUtils.Collision.Object || ImminentCollision == SnakeUtils.Collision.Tail)
                {
                    SnakeImage.Visibility = Visibility.Collapsed;
                    SnakeImage = SnakeUtils.LoadImage("NutsTheme/collision_3_1");
                    CurLevel.LevelCanvas.Children.Add(SnakeImage);
                    SnakeImage.Visibility = Visibility.Visible;
                }
            }
        }

        public void ShowWinner()
        {
            if (Animated)
            {
                _winAnimCount++;

                int frameNo = (_winAnimCount / CurLevel.Animation.WinDelay) % CurLevel.Animation.WinFrames;
                SnakeImage.Visibility = Visibility.Collapsed;
                SnakeImage = AnimationsDict["win"][frameNo];
                SnakeImage.Visibility = Visibility.Visible;

                foreach (SLTail tail in TailList)
                {
                    tail.TailImage.Visibility = Visibility.Collapsed;
                    tail.TailImage = tail.AnimationsDict["win"][frameNo];
                    tail.TailImage.Visibility = Visibility.Visible;
                }

                if (_winAnimCount >= CurLevel.TileSize)
                {
                    _winAnimCount = 0;
                }
         
            } else
            {
                if (_winAnimCount > 0) return;

                SnakeImage.Visibility = Visibility.Collapsed;
                SnakeImage = SnakeUtils.LoadImage(CurLevel.Animation.FilenameWin);
                CurLevel.LevelCanvas.Children.Add(SnakeImage);
                SnakeImage.Visibility = Visibility.Visible;

                foreach (SLTail tail in TailList)
                {
                    tail.TailImage.Visibility = Visibility.Collapsed;
                    tail.TailImage = SnakeUtils.LoadImage(CurLevel.Animation.FilenameWin);
                    CurLevel.LevelCanvas.Children.Add(tail.TailImage);
                    tail.TailImage.Visibility = Visibility.Visible;
                }

                _winAnimCount++;
            }
        }


        public override void SetToStart()
        {

            SnakeUtils.RemoveAnimationInDict(AnimationsDict, "win", CurLevel.LevelCanvas);
            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < CurLevel.Animation.CollisionFrames; x++)
                {
                    _bufferedCollisionImage[x, y].Visibility = Visibility.Collapsed;
                    CurLevel.LevelCanvas.Children.Remove(_bufferedCollisionImage[x, y]);
                }
            }

            foreach (SLTail tail in TailList)
                tail.SetToStart();

            base.SetToStart();

        }

        protected override void AddTail(Tail tail_DC)
        {
            SLTail tail = tail_DC as SLTail;
            base.AddTail(tail);
        }

        protected override Tail CreateTail()
        {
            if (_tailList.Last == null)
                return new SLTail(OldDirection, SnakePosX, SnakePosY, CurLevel);
            else
                return new SLTail(_tailList.Last.Value.OldDirection, _tailList.Last.Value.TailPosX, _tailList.Last.Value.TailPosY, CurLevel);
        }


        private void _UpdateKeyBuffer()
        {
            if (_keyBuffer.Count > 0)
            {
                int index = -1;
                foreach (var dir in _keyBuffer)
                {
                    index++;
                    if (dir != NewDirection)
                    {
                        NewDirection = dir;
                        break;
                    }
                }
                for (int i = 0; i <= index; i++)
                    _keyBuffer.RemoveAt(0);
            }
            HasNewDirectionAssigned = false;
        }

        private void _ClearKeyBuffer()
        {
            _keyBuffer.Clear();
        }

        public void NewDirectionToBuffer(SnakeUtils.Directions newDirection)
        {

            if (!HasNewDirectionAssigned && _keyBuffer.Count == 0)
            {
                NewDirection = newDirection;
                HasNewDirectionAssigned = true;
            }
            else
            {
                _keyBuffer.Add(newDirection);
            }
        }

        public override void OnShrinking(object sender, SnakeEventArgs e)
        {
            LinkedListNode<Tail>[] lastNodes = SnakeUtils.GetLastNodes(TailList, e.Value);

            for (int i = 0; i < lastNodes.Length; i++)
            {
                SLTail tail = (SLTail)lastNodes[i].Value;
                tail.IsShrinking = true;
            }

            base.OnShrinking(sender, e);
        }


        protected override void Shrink()
        {
            LinkedListNode<Tail>[] lastNodes = SnakeUtils.GetLastNodes(TailList, IsShrinking);

            for (int i = 0; i < lastNodes.Length; i++)
            {
                SLTail tail = (SLTail)lastNodes[i].Value;
                tail.TailImage.Visibility = Visibility.Collapsed;
            }

            base.Shrink();
        }


    }
}
