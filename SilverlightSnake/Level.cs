using System;
using System.Collections.Generic;
using System.Windows.Threading;

namespace SilverlightSnake
{
    public class Level
    {
        private bool[,] _apples;
        public bool[,] Apples
        {
          get { return _apples; }
          set { _apples = value; }
        }

        private bool[,] _objects;
        public bool[,] Objects
        {
            get { return _objects; }
            set { _objects = value; }
        }

        private List<Apple> _appleObjects = new List<Apple>();
        public List<Apple> AppleObjects
        {
            get { return _appleObjects; }
            set { _appleObjects = value; }
        }

        private List<LevelObject> _levelObjects = new List<LevelObject>();
        public List<LevelObject> LevelObjects
        {
            get { return _levelObjects; }
            set { _levelObjects = value; }
        }

        public string Name { get; set; }

        public int SnakeStartPosX { get; set; }
        public int SnakeStartPosY { get; set; }

        public int LevelWidth { get; set; }

        public int LevelHeight { get; set; }

        public int CurrentScore { get; set; }

        public int TargetScore { get; set; }

        public int SnakeSpeed { get; set; }

        public bool IsControlInversed { get; set; }

        private Random _rand;
        
        public delegate void ChangedScoreEventHandler(object sender, EventArgs e);

        public event ChangedScoreEventHandler ChangedScoreEvent;

        public delegate void HighScoreEventHandler(object sender, EventArgs e);

        public event HighScoreEventHandler HighScoreEvent;

        public delegate void ChangedSpeedEventHandler(object sender, SnakeEventArgs e);

        public event ChangedSpeedEventHandler ChangedSpeedEvent;

        public delegate void ShrinkingEventHandler(object sender, SnakeEventArgs e);

        public event ShrinkingEventHandler ShrinkingEvent;

        public delegate void InverseEventHandler(object sender, SnakeEventArgs e);

        public event InverseEventHandler InverseEvent;

        private DispatcherTimer speedTimer = new DispatcherTimer();

        private DispatcherTimer inverseTimer = new DispatcherTimer();

        private bool ShowWinApple { get; set; }

        public Level()
        {
            _rand = new Random();
        }

        public virtual void InitializeLevelStart()
        {
            _objects = new bool[LevelWidth, LevelHeight];
            _apples = new bool[LevelWidth, LevelHeight];
        }

        public virtual void InitializeLevelEnd()
        {
            GenerateAllApples(SnakeStartPosX, SnakeStartPosY);
        }

        public virtual void GenerateAllApples(int forbiddenX, int forbiddenY)
        {
            bool[,] forbidden = new bool[LevelWidth, LevelHeight];
            forbidden[forbiddenX, forbiddenY] = true;
            foreach (Apple apple in AppleObjects)
                GenerateApple(forbidden, apple);
        }

        public void GenerateApple(bool[,] forbidden, Apple apple) {
           
            if (apple.IsSet) return;

            if (apple.IsSpecial)
            {
                GenerateSpecialAppleIfNecessary(forbidden, apple);
            }
            else
            {
                FindPositionAndSetApple(forbidden, apple);
            }
        }

        private void GenerateSpecialAppleIfNecessary(bool[,] forbidden, Apple apple)
        {
            int prob = _rand.Next(0, 100);

            if (apple.Speciality.Type.ToLower() == AppleSpeciality.WinType && ShowWinApple == false)
            {
                if (CurrentScore < TargetScore)
                {
                    HideApple(apple);
                }
                else
                {
                    UnhideApple(apple);
                    ShowWinApple = true;
                    FindPositionAndSetApple(forbidden, apple);
                }
            }
            else if (prob >= apple.Speciality.Probability)
            {
                HideApple(apple);
            }
            else
            {
                UnhideApple(apple);
                FindPositionAndSetApple(forbidden, apple);
            }
        }

        private void FindPositionAndSetApple(bool[,] forbidden, Apple apple)
        {
            Position freePosition = FindNextFreePosition(forbidden);
            SetApple(freePosition.X, freePosition.Y, apple);
        }

        private Position FindNextFreePosition(bool[,] forbidden)
        {
            int maxCounter = LevelWidth * LevelHeight;
            Position pos = new Position();
            do
            {
                pos.X = _rand.Next(LevelWidth);
                pos.Y = _rand.Next(LevelHeight);
                maxCounter--;
                if (maxCounter < 0) throw new NoSpaceForApplesException();
            } while (forbidden[pos.X, pos.Y] || Objects[pos.X, pos.Y] || Apples[pos.X, pos.Y]);
            return pos;            
        }
               
        public virtual void RemoveApples()
        {
  
        }

        public virtual void HideApple(Apple specialApple)
        {
            //Hook method to hide apple in display
            ;
        }

        public virtual void UnhideApple(Apple specialApple)
        {
            //Hook method to show apple in display
            ;
        }

        protected virtual void SetApple(int x, int y, Apple apple)
        {
            Apples[x, y] = true;
            apple.ApplePosX = x;
            apple.ApplePosY = y;
            apple.IsSet = true;
        }

        public virtual void EatApple(int x, int y)
        {
            Apples[x, y] = false;
            foreach (Apple apple in AppleObjects)
            {
                if (apple.ApplePosX == x && apple.ApplePosY == y && apple.IsSet)
                {
                    apple.IsSet = false;

                    if (apple.IsSpecial)
                        ThrowEventForSpecialApple(apple);

                    UpdateScore(apple);
                }
            }
        }

        private void ThrowEventForSpecialApple(Apple apple)
        {
            if (apple.Speciality.Type.ToLower() == AppleSpeciality.SpeedingType)
                OnChangedSpeed(new SnakeEventArgs(apple.Speciality.Value, apple.Speciality.Time));
            else if (apple.Speciality.Type.ToLower() == AppleSpeciality.ShrinkingType)
                OnShrinkingEvent(new SnakeEventArgs(apple.Speciality.Value, 0));
            else if (apple.Speciality.Type.ToLower() == AppleSpeciality.InverseType)
                OnInverseEvent(new SnakeEventArgs(0, apple.Speciality.Time));
            else if (apple.Speciality.Type.ToLower() == AppleSpeciality.WinType)
                OnHighScore(new EventArgs());
        }

        private void UpdateScore(Apple apple)
        {
            if (apple.Points != 0)
            {
                CurrentScore += apple.Points;
                if (CurrentScore < 0) CurrentScore = 0;
                OnChangedScore(new EventArgs());
            }
        }

        public virtual void OnChangedSpeed(SnakeEventArgs e)
        {
            speedTimer.Stop();
            if (e.Value != this.SnakeSpeed)
            {
                speedTimer.Interval = new TimeSpan(0, 0, 0, e.Time);
                speedTimer.Tick += new EventHandler(speedTimer_Tick);
                speedTimer.Start();
            }

            if (ChangedSpeedEvent != null) 
                ChangedSpeedEvent(this, e);

        }

        public virtual void OnShrinkingEvent(SnakeEventArgs e)
        {
            if (ShrinkingEvent != null)
                ShrinkingEvent(this, e);
        }

        public virtual void OnInverseEvent(SnakeEventArgs e)
        {
            IsControlInversed = true;
            inverseTimer.Interval = new TimeSpan(0, 0, 0, e.Time);
            inverseTimer.Tick += new EventHandler(inverseTimer_Tick);
            inverseTimer.Start();

            if (InverseEvent != null)
                InverseEvent(this, e);
        }

        private void speedTimer_Tick(object Sender, EventArgs e)
        {
            OnChangedSpeed(new SnakeEventArgs(this.SnakeSpeed, 0));
            speedTimer.Stop();
        }

        private void inverseTimer_Tick(object Sender, EventArgs e)
        {
            IsControlInversed = false;
            inverseTimer.Stop();

        }

        public virtual void OnChangedScore(EventArgs e)
        {
            if (ChangedScoreEvent != null)
                ChangedScoreEvent(this, e);
        }

        public virtual void OnHighScore(EventArgs e)
        {
            if (HighScoreEvent != null)
                HighScoreEvent(this, e);
        }
    }
}
