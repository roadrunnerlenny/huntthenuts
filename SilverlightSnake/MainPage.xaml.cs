using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Xml;

namespace SilverlightSnake
{
    public partial class MainPage : UserControl
    {
        #region Private Vars

        private SLLevel level1SL;
        private SLSnake snakeSL;
        private bool isGameOver = true;
        private bool isPause = false;
        private bool hasWon = false;
        private byte levelNr = 1;
        private bool isJustCreated = true;
        private DispatcherTimer dt = new DispatcherTimer();
        private DispatcherTimer delayTimer = new DispatcherTimer();
        private byte _animCount = 0;

        private enum MenuButton
        {
            None, Restart, NextLevel, Retire
        }

        private MenuButton _showButton = MenuButton.None;

        #endregion

        #region Properties

        public MainPresenter Presenter { get; set; }

        #endregion
       
        #region Main Render Logic

        public MainPage()
        {
            InitializeComponent();
            CompositionTarget.Rendering += new EventHandler(MainRenderLoop);

            Presenter = new MainPresenter();
            this.DataContext = Presenter;
        }

        void MainRenderLoop(object sender, EventArgs e)
        {
            try
            {
                snakeSL.SnakeImage.SetValue(Canvas.TopProperty, snakeSL.Pos.Y + level1SL.SnakeOffsetY);
                snakeSL.SnakeImage.SetValue(Canvas.LeftProperty, snakeSL.Pos.X);
                foreach (Tail tail in snakeSL.TailList)
                {
                    SLTail ntail = (SLTail)tail;
                    ntail.TailImage.SetValue(Canvas.TopProperty, ntail.Pos.Y + level1SL.SnakeOffsetY);
                    ntail.TailImage.SetValue(Canvas.LeftProperty, ntail.Pos.X);
                }
            }

            catch (Exception)
            {
                //should never happen - if it does, no heck of an idea what to do... Perhaps reload the page?
                Presenter.MessageText = "Ein neuer Fehler traf auf :( ";
            }
        }

        void dt_Tick(object Sender, EventArgs e)
        {
            //This is the main game loop
            if (isGameOver == false && isPause == false)
            {
                try
                {
                    snakeSL.MoveSL();
                }
                catch (CollisionException hit)
                {
                    isGameOver = true;
                    if (hit.GetType().Equals(typeof(CollisionWallHitException)))
                        Presenter.MessageText = hit.ToString(); _animateMessageBox();

                    if (hit.GetType().Equals(typeof(CollisionHitItselfException)))
                        Presenter.MessageText = hit.ToString(); _animateMessageBox();

                    if (hit.GetType().Equals(typeof(CollisionHitObjectException)))
                        Presenter.MessageText = hit.ToString(); _animateMessageBox();

                    _showStartMenu();
                }
                catch (NoSpaceForApplesException noSpace)
                {
                    //The user should get a bonus - this should never happen!
                    isGameOver = true;
                    Presenter.MessageText = noSpace.ToString(); _animateMessageBox();
                }
            }
            else if (hasWon == true)
            {
                snakeSL.ShowWinner();
            }
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (!level1SL.IsControlInversed)
            {
                switch (e.Key)
                {
                    case Key.Up: snakeSL.NewDirectionToBuffer(SnakeUtils.Directions.Up);
                        break;
                    case Key.Right: snakeSL.NewDirectionToBuffer(SnakeUtils.Directions.Right);
                        break;
                    case Key.Down: snakeSL.NewDirectionToBuffer(SnakeUtils.Directions.Down);
                        break;
                    case Key.Left: snakeSL.NewDirectionToBuffer(SnakeUtils.Directions.Left);
                        break;
                    default: break;
                }
            }
            else
            {
                switch (e.Key)
                {
                    case Key.Up: snakeSL.NewDirectionToBuffer(SnakeUtils.Directions.Down);
                        break;
                    case Key.Right: snakeSL.NewDirectionToBuffer(SnakeUtils.Directions.Left);
                        break;
                    case Key.Down: snakeSL.NewDirectionToBuffer(SnakeUtils.Directions.Up);
                        break;
                    case Key.Left: snakeSL.NewDirectionToBuffer(SnakeUtils.Directions.Right);
                        break;
                    default: break;
                }
            }
        }

        #endregion

        #region Initialization

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            delayTimer.Tick += new EventHandler(delayTimer_Tick);

            _loadLevel("Level" + levelNr);
            _setGameFieldSize();
            _addSpeedControl();
            _showStartMenu();
            //_testRun();
        }

        private void _loadLevel(string levelName)
        {
            //Initialize Level and Snake
            try
            {
                if (level1SL != null)
                {
                    level1SL.ChangedScoreEvent -= ChangedScore;
                    level1SL.HighScoreEvent -= HighScore;
                    level1SL.ChangedSpeedEvent -= ChangedSpeed;
                }
                level1SL = SLLevelBuilder.GenerateLevel(levelName);
            }
            catch (FormatException)
            {
                //This error occurs when in the Level xml file text data is set where numbers are expected
                Presenter.MessageText = "Fehler beim Lesen der Leveldaten!";
                return;
            }
            catch (IndexOutOfRangeException)
            {
                //This error occurs when in the Level xml file invalid positions are set 
                //(e.g. X,Y:11,12 as snake starting position, while the level has a size of 10x10)
                Presenter.MessageText = "Ungültige Level Definition";
                return;
            }
            catch (XmlException)
            {
                //This error occurs when the Level file can't be opened
                Presenter.MessageText = "Level konnte nicht geöffnet werden";
            }
            snakeSL = new SLSnake(level1SL);

            GameField.Children.Add(level1SL.LevelCanvas);

            level1SL.ChangedScoreEvent += new Level.ChangedScoreEventHandler(ChangedScore);
            level1SL.HighScoreEvent += new Level.HighScoreEventHandler(HighScore);
            level1SL.ChangedSpeedEvent += new SLLevel.ChangedSpeedEventHandler(ChangedSpeed);

            Presenter.MessageText = "Ziel sind " + level1SL.TargetScore + " Nüsse";

        }

        private void _unloadLevel()
        {
            GameField.Children.Remove(level1SL.LevelCanvas);
        }

        private void _setGameFieldSize()
        {
            GameField.Width = level1SL.LevelWidth * level1SL.TileSize;
            GameField.Height = level1SL.LevelHeight * level1SL.TileSize;
            BorderGameField.Width = level1SL.LevelWidth * level1SL.TileSize + level1SL.TileSize * 2;
            BorderGameField.Height = level1SL.LevelHeight * level1SL.TileSize + level1SL.TileSize * 2;
            BorderGameField.BorderBrush = level1SL.BorderBackground;
            BorderGameField.BorderThickness = new Thickness((double)level1SL.TileSize);
        }

        private void _addSpeedControl()
        {

            //Initialize Speed Control Thread
            dt.Interval = new TimeSpan(0, 0, 0, 0, level1SL.SnakeSpeed);
            dt.Tick += new EventHandler(dt_Tick);
            dt.Start();
        }

        private void _restartSpeedControl()
        {
            dt.Stop();
            dt.Tick -= dt_Tick;
            dt.Interval = new TimeSpan(0, 0, 0, 0, level1SL.SnakeSpeed);
            dt.Tick += new EventHandler(dt_Tick);
            dt.Start();
        }

        #endregion

        #region Animations

        private void _animateMessageBox()
        {
            messageLabel.Foreground = new SolidColorBrush(Colors.Red);
            DoubleAnimation dAnimation = new DoubleAnimation();
            dAnimation.From = 14;
            dAnimation.To = 16;
            dAnimation.Duration = new Duration(SnakeUtils.MessageLabelAnimation);
            dAnimation.AutoReverse = true;

            Storyboard story = new Storyboard();
            story.Completed += new EventHandler(story_Completed);
            story.Children.Add(dAnimation);
            Storyboard.SetTarget(dAnimation, messageLabel);
            Storyboard.SetTargetProperty(dAnimation, new PropertyPath(TextBlock.FontSizeProperty));

            Buttons.SetValue(Canvas.ZIndexProperty, 999);
            Buttons.Visibility = Visibility.Visible;
            _animCount++;
            story.Begin();
        }

        private void _animateButtons(MenuButton button)
        {
            _showButton = button;
            delayTimer.Interval = SnakeUtils.ButtonAnimationDelay;
            delayTimer.Tick += new EventHandler(delayTimer_Tick);
            delayTimer.Start();
        }

        void delayTimer_Tick(object sender, EventArgs e)
        {
            delayTimer.Stop();

            DoubleAnimation dAnimation = new DoubleAnimation();
            dAnimation.From = 0;
            dAnimation.To = (double)Buttons.Width;
            dAnimation.Duration = new Duration(SnakeUtils.ButtonAnimation);
            dAnimation.AutoReverse = false;

            Storyboard story = new Storyboard();
            story.Children.Add(dAnimation);
            Storyboard.SetTarget(dAnimation, Buttons);
            Storyboard.SetTargetProperty(dAnimation, new PropertyPath(StackPanel.WidthProperty));

            Buttons.SetValue(Canvas.ZIndexProperty, 999);
            Buttons.Visibility = Visibility.Visible;
            switch (_showButton)
            {
                case MenuButton.NextLevel: NextLevelButton.Visibility = Visibility.Visible; break;
                case MenuButton.Retire: RetireButton.Visibility = Visibility.Visible; break;
                case MenuButton.Restart: Restartbutton.Visibility = Visibility.Visible; break;
            }
            _showButton = MenuButton.None;
            story.Begin();
        }

        void story_Completed(object sender, EventArgs e)
        {
            if (_animCount == 1) messageLabel.Foreground = new SolidColorBrush(Colors.Black);
            _animCount--;
        }

        #endregion

        #region Process Events

        private void ChangedSpeed(object sender, SnakeEventArgs e)
        {
            dt.Interval = new TimeSpan(0, 0, 0, 0, e.Value);
        }

        private void ChangedScore(object sender, EventArgs e)
        {
            ScoreDisplay.Text = level1SL.CurrentScore.ToString();
        }

        private void HighScore(object sender, EventArgs e)
        {
            isGameOver = true;
            hasWon = true;
            PauseButton.IsEnabled = false;

            Buttons.Visibility = Visibility.Collapsed;
            if (levelNr < SnakeUtils.Levels)
            {
                Presenter.MessageText = "Geschafft! Gratuliere!"; _animateMessageBox();
                _animateButtons(MenuButton.NextLevel);
            }
            else
            {
                Presenter.MessageText = "Alle Level geschafft! Yippie!"; _animateMessageBox();
                _animateButtons(MenuButton.Retire);
            }
        }

        #endregion

        #region Process Buttons

        private void Restartbutton_Click(object sender, RoutedEventArgs e)
        {
            isGameOver = false;
            Buttons.Visibility = Visibility.Collapsed;
            Restartbutton.Visibility = Visibility.Collapsed;

            if (!isJustCreated)
            {
                snakeSL.SetToStart();
                _loadLevel("Level" + levelNr);
            }
            else
            {
                isJustCreated = false;
            }
            Presenter.MessageText = "Ziel sind " + level1SL.TargetScore + " Nüsse";
            _animateMessageBox();
            ScoreDisplay.Text = level1SL.CurrentScore.ToString();
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            isPause = true;
            PauseButton.Visibility = Visibility.Collapsed;
            GoButton.Visibility = Visibility.Visible;
        }

        private void GoButton_Click(object sender, RoutedEventArgs e)
        {
            isPause = false;
            GoButton.Visibility = Visibility.Collapsed;
            PauseButton.Visibility = Visibility.Visible;
        }

        private void NextLevelButton_Click(object sender, RoutedEventArgs e)
        {
            hasWon = false;
            isGameOver = false;
            _unloadLevel();
            levelNr++;
            _loadLevel("Level" + levelNr);
            _setGameFieldSize();
            PauseButton.IsEnabled = true;
            _restartSpeedControl();
            level1SL.OnChangedScore(new EventArgs());

            Buttons.Visibility = Visibility.Collapsed;
            NextLevelButton.Visibility = Visibility.Collapsed;
        }

        private void RetireButton_Click(object sender, RoutedEventArgs e)
        {
            Buttons.Visibility = Visibility.Collapsed;
            RetireButton.Visibility = Visibility.Collapsed;
            GameField.Children.Clear();
            Canvas retireCanvas = new Canvas();
            retireCanvas.Background = SnakeUtils.LoadBackgroundImage(SnakeUtils.RetireFile);
            retireCanvas.Width = 200.0;
            retireCanvas.Height = 200.0;
            GameField.Children.Add(retireCanvas);
        }

        private void _showStartMenu()
        {
            Buttons.Visibility = Visibility.Collapsed;
            _animateButtons(MenuButton.Restart);
        }

        #endregion

        #region Testrun

        delegate void MoveSnakeXTimes(int callMove);

        private void _testRun()
        {
            Level level = LevelBuilder.GenerateLevel("Level1");
            Snake snake = new Snake(level);

            MoveSnakeXTimes moveSnake = delegate(int callMove)
            {
                for (int i = 0; i < callMove; i++)
                    snake.Move();
            };

            snake.NewDirection = SnakeUtils.Directions.Right;
            snake.ChangeDirection();
            moveSnake(6);
            snake.NewDirection = SnakeUtils.Directions.Down;
            snake.ChangeDirection();
            moveSnake(3);
        }

        #endregion

    }
}
