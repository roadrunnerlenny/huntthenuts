using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace SilverlightSnake
{
    public class SLLevel : Level
    {
        #region SLLevel public attributes

        public double SnakeOffsetY { get; set; }

        public string FilenameHead { get; set; }

        public bool AnimatedSnake { get; set; }

        //Needed for BufferedAnimation
        public SLAnimation Animation = new SLAnimation();

        private Canvas _levelCanvas;
        public Canvas LevelCanvas
        {
            get { return _levelCanvas; }
            set { _levelCanvas = value; }
        }

        public Brush BorderBackground { get; set; }

        public int TileSize { get; set; }

        private Storyboard story;

        #endregion

        #region SLLevel protected functions

        private void _doGrowAnimation(SLApple apple) 
        {
            DoubleAnimation dAnimation = new DoubleAnimation();
            dAnimation.From = 0;
            dAnimation.To = (double)TileSize;
            dAnimation.Duration = new Duration(SnakeUtils.GrowTime);
            dAnimation.AutoReverse = false;

            story = new Storyboard();
            story.Children.Add(dAnimation);
            Storyboard.SetTarget(dAnimation, apple.AppleImage);
            Storyboard.SetTargetProperty(dAnimation, new PropertyPath(Image.WidthProperty));

            story.Begin();
        }
       
        protected override void SetApple(int x, int y, Apple apple_DC)
        {
            SLApple apple = apple_DC as SLApple;

            base.SetApple(x, y, apple);
            apple.Pos = new Point((double)apple.ApplePosX * TileSize, (double)apple.ApplePosY * TileSize);
            apple.AppleImage.SetValue(Canvas.LeftProperty, apple.Pos.X);
            apple.AppleImage.SetValue(Canvas.TopProperty, apple.Pos.Y + apple.AppleOffsetY);

            _doGrowAnimation(apple);

        }

        #endregion

        #region public functions

        public override void HideApple(Apple specialApple_DC)
        {
            SLApple specialApple = specialApple_DC as SLApple;
            specialApple.AppleImage.Visibility = Visibility.Collapsed;
        }

        public override void UnhideApple(Apple specialApple_DC)
        {
            SLApple specialApple = specialApple_DC as SLApple;
            specialApple.AppleImage.Visibility = Visibility.Visible;
            
        }

        public override void RemoveApples()
        {
            foreach (SLApple apple in AppleObjects)
            {
                apple.AppleImage.Visibility = Visibility.Collapsed;
                LevelCanvas.Children.Remove(apple.AppleImage);
            }

            foreach (SLLevelObject levelObject in LevelObjects)
            {
                levelObject.ObjectImage.Visibility = Visibility.Collapsed;
                LevelCanvas.Children.Remove(levelObject.ObjectImage);
            }

            base.RemoveApples();
        }

        public override void InitializeLevelStart()
        {
            base.InitializeLevelStart();
        }

        public override void InitializeLevelEnd()
        {
            base.InitializeLevelEnd();
        }

        #endregion
    }
}
