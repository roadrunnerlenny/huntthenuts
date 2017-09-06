using System;
using System.Windows;
using System.Windows.Controls;
using System.Xml;


namespace SilverlightSnake
{
    public class SLLevelBuilder : LevelBuilder
    {
        protected SLLevelBuilder()
        {
            //just to make the constructor protected;
        }

        public static new SLLevel GenerateLevel(string levelname)
        {
            SLLevel creationLevel = new SLLevel();

            SLLevelBuilder builder = new SLLevelBuilder();

            Canvas levelCanvas = new Canvas();
            creationLevel.LevelCanvas = levelCanvas;

            builder.ReadLevel(creationLevel, levelname);          

            return creationLevel;
        }

        protected override LevelObject CreateLevelObject()
        {
            return new SLLevelObject();
        }

        protected override void ReadLevelTag(Level creationLevel_DC, XmlReader reader)
        {
            SLLevel creationLevel = creationLevel_DC as SLLevel;
            base.ReadLevelTag(creationLevel, reader);

            reader.MoveToAttribute("tileSize");
            creationLevel.TileSize = Int32.Parse(reader.Value);

            creationLevel.LevelCanvas.Width = (double)(creationLevel.LevelWidth * creationLevel.TileSize);
            creationLevel.LevelCanvas.Height = (double)(creationLevel.LevelHeight * creationLevel.TileSize);

            reader.MoveToAttribute("background");
            creationLevel.LevelCanvas.Background = SnakeUtils.LoadBackgroundImage(reader.Value);

            reader.MoveToAttribute("borderBackground");
            creationLevel.BorderBackground = SnakeUtils.LoadBackgroundImage(reader.Value);
            
        }

        protected override void ReadSnakeTag(Level creationLevel_DC, XmlReader reader)
        {
            SLLevel creationLevel = creationLevel_DC as SLLevel;
            base.ReadSnakeTag(creationLevel, reader);

            reader.MoveToAttribute("offsetY");
            creationLevel.SnakeOffsetY = Double.Parse(reader.Value);

            reader.MoveToAttribute("filenameHead");
            creationLevel.FilenameHead = reader.Value;

            reader.MoveToAttribute("filenameTail");
            creationLevel.Animation.FilenameTail = reader.Value;

            reader.MoveToAttribute("filenameCollision");
            creationLevel.Animation.FilenameCollision = reader.Value;

            reader.MoveToAttribute("filenameWin");
            creationLevel.Animation.FilenameWin = reader.Value;

            reader.MoveToAttribute("filenameGrow");
            creationLevel.Animation.FilenameGrow = reader.Value;

            reader.MoveToAttribute("filenameShrink");
            creationLevel.Animation.FilenameShrink = reader.Value;

            reader.MoveToAttribute("animated");
            creationLevel.AnimatedSnake = Boolean.Parse(reader.Value);

            reader.MoveToAttribute("animationFrames");
            creationLevel.Animation.AnimationFrames= Byte.Parse(reader.Value);

            reader.MoveToAttribute("collisionFrames");
            creationLevel.Animation.CollisionFrames = Byte.Parse(reader.Value);

            reader.MoveToAttribute("winFrames");
            creationLevel.Animation.WinFrames = Byte.Parse(reader.Value);

            reader.MoveToAttribute("growFrames");
            creationLevel.Animation.GrowFrames = Byte.Parse(reader.Value);

            reader.MoveToAttribute("shrinkFrames");
            creationLevel.Animation.ShrinkFrames = Byte.Parse(reader.Value);

            reader.MoveToAttribute("animationDelay");
            creationLevel.Animation.AnimationDelay = Byte.Parse(reader.Value);

            reader.MoveToAttribute("collisionDelay");
            creationLevel.Animation.CollisionDelay = Byte.Parse(reader.Value);

            reader.MoveToAttribute("winDelay");
            creationLevel.Animation.WinDelay = Byte.Parse(reader.Value);

            reader.MoveToAttribute("growDelay");
            creationLevel.Animation.GrowDelay = Byte.Parse(reader.Value);

            reader.MoveToAttribute("shrinkDelay");
            creationLevel.Animation.ShrinkDelay = Byte.Parse(reader.Value);

        }

        protected override Apple CreateAppleObject()
        {
            return new SLApple();
        }


        protected override void ReadAppleTag(Level creationLevel_DC, XmlReader reader, Apple apple_DC)
        {
            SLLevel creationLevel = creationLevel_DC as SLLevel;
            SLApple apple = apple_DC as SLApple;

            base.ReadAppleTag(creationLevel, reader, apple);

            reader.MoveToAttribute("img");
            string appleFilename = reader.Value;

            reader.MoveToAttribute("offsetY");
            apple.AppleOffsetY = Double.Parse(reader.Value);

            apple.AppleImage = SnakeUtils.LoadImage(appleFilename);
            creationLevel.LevelCanvas.Children.Add(apple.AppleImage);
        }

        protected override void ReadObjectTag(Level creationLevel_DC, XmlReader reader, LevelObject levObject_DC)
        {
            SLLevelObject levObject = levObject_DC as SLLevelObject;
            SLLevel creationLevel = creationLevel_DC as SLLevel;

            base.ReadObjectTag(creationLevel_DC, reader, levObject_DC);

            levObject.Pos = new Point((double)levObject.ObjectPosX * creationLevel.TileSize, (double)levObject.ObjectPosY * creationLevel.TileSize);

            reader.MoveToAttribute("offsetY");
            levObject.OffsetY = Double.Parse(reader.Value);

            reader.MoveToAttribute("filename");
            levObject.ObjectImage = SnakeUtils.LoadImage(reader.Value);

            levObject.ObjectImage.SetValue(Canvas.LeftProperty, levObject.Pos.X);
            levObject.ObjectImage.SetValue(Canvas.TopProperty, levObject.Pos.Y + levObject.OffsetY);
            creationLevel.LevelCanvas.Children.Add(levObject.ObjectImage);
        }
    }
}
