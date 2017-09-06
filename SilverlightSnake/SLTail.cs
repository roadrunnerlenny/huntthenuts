using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Media.Animation;

namespace SilverlightSnake
{
    public class SLTail : Tail
    {
        private Point _pos;
        public Point Pos
        {
            get { return _pos; }
            set { _pos = value; }
        }

        public Image TailImage { get; set; }

        private Image[,] _bufferedImage;

        public Image[,] BufferedImage
        {
            get { return _bufferedImage; }
            set { _bufferedImage = value; }
        }

        private Dictionary<string, Image[]> _animationsDict = new Dictionary<string,Image[]>();

        public Dictionary<string, Image[]> AnimationsDict
        {
            get { return _animationsDict; }
            set { _animationsDict = value; }
        }

        public new SLLevel CurLevel { get; set; }

        public bool IsShrinking { get; set; }

        public void SLMoveTail(LinkedList<Tail> tailList)
        {
            SnakeUtils.MoveSLPosition(_oldDirection, ref _pos, 1.0);
        }

        private void _initAnimation()
        {
            _bufferedImage = new Image[CurLevel.Animation.AnimationFrames, 4];

            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < CurLevel.Animation.AnimationFrames; x++)
                {
                    _bufferedImage[x, y] = SnakeUtils.LoadImage(CurLevel.Animation.FilenameTail + "_" + (x + 1) + "_" + (y + 1));
                    _bufferedImage[x, y].Visibility = Visibility.Collapsed;
                    CurLevel.LevelCanvas.Children.Add(_bufferedImage[x, y]);
                }
            }

            TailImage = BufferedImage[0, 0];
            TailImage.Visibility = Visibility.Visible;
        }


        public void Animate(int tileCount)
        {
            TailImage.Visibility = Visibility.Collapsed;
            if (IsGrowing)
            {
                int frameNo = (tileCount / CurLevel.Animation.GrowDelay) % CurLevel.Animation.GrowFrames;
                TailImage = AnimationsDict["grow"][frameNo];
            } else if (IsShrinking)
            {
                int frameNo = (tileCount / CurLevel.Animation.ShrinkDelay) % CurLevel.Animation.ShrinkFrames;
                TailImage = AnimationsDict["shrink"][frameNo];
            } else
            {
                int frameNo = (tileCount / CurLevel.Animation.AnimationDelay) % CurLevel.Animation.AnimationFrames;
                TailImage = BufferedImage[frameNo, ((int)this.OldDirection - 1)];
            }
            TailImage.Visibility = Visibility.Visible;
        }

        public SLTail(SnakeUtils.Directions direction, int tailPosX, int tailPosY, SLLevel curLevel)
            : base(direction, tailPosX, tailPosY, curLevel)
        {
            CurLevel = curLevel;
            _pos.X = (double)(tailPosX * CurLevel.TileSize);
            _pos.Y = (double)(tailPosY * CurLevel.TileSize);

            switch (direction)
            {
                case SnakeUtils.Directions.Up: _pos.Y += CurLevel.TileSize;
                    break;
                case SnakeUtils.Directions.Right: _pos.X -= CurLevel.TileSize;
                    break;
                case SnakeUtils.Directions.Down: _pos.Y -= CurLevel.TileSize;
                    break;
                case SnakeUtils.Directions.Left: _pos.X += CurLevel.TileSize;
                    break;
            }


            if (CurLevel.AnimatedSnake)
            {
                _initAnimation();
                SnakeUtils.InitAnimationInDict(AnimationsDict, "win", CurLevel.Animation.FilenameWin, CurLevel.Animation.WinFrames, CurLevel.LevelCanvas);
                SnakeUtils.InitAnimationInDict(AnimationsDict, "grow", CurLevel.Animation.FilenameGrow, CurLevel.Animation.GrowFrames, CurLevel.LevelCanvas);
                SnakeUtils.InitAnimationInDict(AnimationsDict, "shrink", CurLevel.Animation.FilenameShrink, CurLevel.Animation.ShrinkFrames, CurLevel.LevelCanvas);
            } 
            else
            {
                TailImage = SnakeUtils.LoadImage(curLevel.Animation.FilenameTail);
                CurLevel.LevelCanvas.Children.Add(TailImage);
            }

            TailImage.SetValue(Canvas.LeftProperty, Pos.X);
            TailImage.SetValue(Canvas.TopProperty, Pos.Y + CurLevel.SnakeOffsetY);

        }

        public override void SetToStart()
        {
            SnakeUtils.RemoveAnimationInDict(AnimationsDict, "win", CurLevel.LevelCanvas);
            SnakeUtils.RemoveAnimationInDict(AnimationsDict, "grow", CurLevel.LevelCanvas);
            SnakeUtils.RemoveAnimationInDict(AnimationsDict, "shrink", CurLevel.LevelCanvas);

            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < CurLevel.Animation.AnimationFrames; x++)
                {
                    _bufferedImage[x, y].Visibility = Visibility.Collapsed;
                    CurLevel.LevelCanvas.Children.Remove(_bufferedImage[x, y]);
                }
            }

            base.SetToStart();
        }
    }
}
