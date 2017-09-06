using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SilverlightSnake
{
    public class SnakeUtils
    {
        private const byte _levels = 4;

        public static TimeSpan GrowTime = TimeSpan.FromSeconds(1);
        public static TimeSpan ButtonAnimation = TimeSpan.FromSeconds(1);
        public static TimeSpan MessageLabelAnimation = TimeSpan.FromMilliseconds(800);
        public static TimeSpan ButtonAnimationDelay = TimeSpan.FromMilliseconds(1500);

        public static string RetireFile = "NutsTheme/retired.jpg";

        /// <summary>
        /// Total Number of Levels
        /// </summary>
        public static byte Levels
        {
            get { return SnakeUtils._levels; }
        }

        private static string _imgDir = "/Content/";
        /// <summary>
        /// Path to /Content-Directory in Silverlight application.
        /// </summary>
        public static string ImgDir
        {
            get
            {
                return _imgDir;
            }
        }

        private static string _levelDir = "Levels/";
        /// <summary>
        /// Path to Level-Directory
        /// </summary>
        public static string LevelDir
        {
            get
            {
                return _levelDir;
            }
        }

        /// <summary>
        /// Possible Snake-Directions: Stop, Up, Right, Down, Left. 
        /// </summary>
        public enum Directions : int
        {
            Stop, Up, Right, Down, Left
        }

        public enum Collision : int
        {
            Nothing, Wall, Tail, Object, Apple
        }


        /// <summary>
        /// Returns an image object from a png file. The file must be located in the /Content-Directory
        /// </summary>
        /// <param name="resourceName">The filename of the png image.</param>
        /// <returns>Image object of the loaded png file</returns>
        public static Image LoadImage(string resourceName)
        {
            Uri filename;
            if (resourceName.EndsWith(".png") || resourceName.EndsWith(".jpg"))
                filename = new Uri(ImgDir+resourceName, UriKind.Relative);
            else
                filename = new Uri(ImgDir+resourceName + ".png", UriKind.Relative);

            BitmapImage bmp = new BitmapImage(filename);
            
            Image i = new Image();
            i.Source = bmp;

            return i;
        }

        /// <summary>
        /// Returns a Brush created from a png file. The file must be located in the /Content-Directory. 
        /// </summary>
        /// <param name="resourceName">The filename of the png image.</param>
        /// <returns>ImageBrush object of the loaded png file.</returns>
        public static ImageBrush LoadBackgroundImage(string resourceName)
        {
            ImageBrush background = new ImageBrush();
            background.ImageSource = (BitmapImage)(LoadImage(resourceName)).Source;
            return background;
        }

        /// <summary>
        /// The Position Coordinates are changed - e.g. the Y-Value is increased by 1 if the 
        /// passed direction value is Down and the passed pixel value is 1.
        /// </summary>
        /// <param name="direction">The direction indicates if either the X or Y value of the position is increased or decreased </param>
        /// <param name="imgPos">The position coordinates that are changed</param>
        /// <param name="pixel">The value with which the X or Y value is increased or decreased</param>
        public static void MoveSLPosition(Directions direction, ref Point imgPos, double pixel)
        {
            switch (direction)
            {
                case SnakeUtils.Directions.Up: imgPos.Y -= pixel;
                    break;
                case SnakeUtils.Directions.Right: imgPos.X += pixel;
                    break;
                case SnakeUtils.Directions.Down: imgPos.Y += pixel;
                    break;
                case SnakeUtils.Directions.Left: imgPos.X -= pixel;
                    break;
            }
        }

        /// <summary>
        /// /// Set z-Level to let Snake and Tail-Objects appear "behind objects"
        /// </summary>
        /// <param name="curLevel">The current Level object</param>
        /// <param name="snake">Snake object</param>
        /// <param name="tailList">Linked List containing all tail objects</param>
        public static void SetZIndex(SLLevel curLevel, SLSnake snake, LinkedList<Tail> tailList)
        {
            //Check for all apples
            foreach (SLApple apple in curLevel.AppleObjects)
            {
                apple.AppleImage.SetValue(Canvas.ZIndexProperty, apple.ApplePosY);
            }

            //Check for all Objects in Level
            foreach (SLLevelObject levelObject in curLevel.LevelObjects)
            {
                levelObject.ObjectImage.SetValue(Canvas.ZIndexProperty, levelObject.ObjectPosY);
            }

            //Check for the tail of snake
            foreach (SLTail tail in tailList)
            {
                tail.TailImage.SetValue(Canvas.ZIndexProperty, tail.TailPosY);
                if (curLevel.AnimatedSnake)
                {
                    for (int y = 0; y < 4; y++)
                        for (int x = 0; x < curLevel.Animation.AnimationFrames; x++)
                            tail.BufferedImage[x, y].SetValue(Canvas.ZIndexProperty, tail.TailPosY);
                }
            }

            snake.SnakeImage.SetValue(Canvas.ZIndexProperty,snake.SnakePosY);
            if (curLevel.AnimatedSnake)
            {
                for (int y = 0; y < 4; y++)
                    for (int x = 0; x < curLevel.Animation.AnimationFrames; x++)
                        snake.BufferedImage[x, y].SetValue(Canvas.ZIndexProperty, snake.SnakePosY);
            }
        }
 
        public static double CalcAnimationOffsetY(SLLevel curLevel, SnakeUtils.Directions direction)
        {
            double animationOffsetY = 0.0;
            switch (direction)
            {
                case SnakeUtils.Directions.Down: animationOffsetY = 0.0; break;
                case SnakeUtils.Directions.Left: animationOffsetY = (double)(curLevel.TileSize - curLevel.SnakeOffsetY); break;
                case SnakeUtils.Directions.Right: animationOffsetY = (double)2 * (curLevel.TileSize - curLevel.SnakeOffsetY); break;
                case SnakeUtils.Directions.Up: animationOffsetY = (double)3 * (curLevel.TileSize - curLevel.SnakeOffsetY); break;
            }
            return animationOffsetY;
        }

        public static void InitAnimationInDict(Dictionary<string,Image[]> AnimationsDict, string key, string filename, int frames, Canvas levelCanvas)
        {
            Image[] newImages = new Image[frames];
            for (int x = 0; x < frames; x++)
            {
                newImages[x] = SnakeUtils.LoadImage(filename + "_" + (x + 1));
                newImages[x].Visibility = Visibility.Collapsed;
                levelCanvas.Children.Add(newImages[x]);
            }
            AnimationsDict.Add(key, newImages);
        }

        public static void RemoveAnimationInDict(Dictionary<string, Image[]> AnimationsDict, string key, Canvas levelCanvas)
        {
            Image[] delImages = AnimationsDict[key];
            for (int x = 0; x < delImages.Length; x++)
            {
                delImages[x].Visibility = Visibility.Collapsed;
                levelCanvas.Children.Remove(delImages[x]);
            }
        }

        public static LinkedListNode<Tail>[] GetLastNodes(LinkedList<Tail> TailList, int nodes)
        {
            List<LinkedListNode<Tail>> lastNodes = new List<LinkedListNode<Tail>>();

            if (TailList.Last != null)
            {
                LinkedListNode<Tail> lastNode = TailList.Last;
                lastNodes.Add(lastNode);
                int count = 1;
                while (count < nodes)
                {
                    lastNode = lastNode.Previous;
                    if (lastNode != null)
                    {
                        lastNodes.Add(lastNode);
                    } else 
                    {
                        break;
                    }
                    count++;
                }
            }

            return lastNodes.ToArray();
        }
    }
}
