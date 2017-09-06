using System;
using System.Xml;

namespace SilverlightSnake
{
    public class LevelBuilder
    {
        protected LevelBuilder()
        {
            //just to make the constructor protected;
        }

        public static Level GenerateLevel(string levelname)
        {
            Level creationLevel = new Level();

            LevelBuilder builder = new LevelBuilder();

            builder.ReadLevel(creationLevel, levelname);

            return creationLevel;
        }

        protected void ReadLevel(Level creationLevel, string levelname)
        {            
            using (XmlReader reader = XmlReader.Create(SnakeUtils.LevelDir + levelname + ".xml"))
            {                
                reader.ReadToFollowing("level");
                ReadLevelTag(creationLevel, reader);

                creationLevel.InitializeLevelStart();

                reader.ReadToFollowing("snake");
                ReadSnakeTag(creationLevel, reader);

                reader.ReadToFollowing("apple");
                while (reader.HasAttributes)
                {
                    Apple apple = CreateAppleObject();
                    creationLevel.AppleObjects.Add(apple);
                    ReadAppleTag(creationLevel, reader, apple);
                    reader.ReadToNextSibling("apple");
                }

                reader.ReadToFollowing("object");
                while (reader.HasAttributes)
                {
                    LevelObject levObject = CreateLevelObject();
                    creationLevel.LevelObjects.Add(levObject);
                    ReadObjectTag(creationLevel, reader, levObject);
                    reader.ReadToNextSibling("object");
                }               

                creationLevel.InitializeLevelEnd();
            }
        }

        protected virtual LevelObject CreateLevelObject()
        {
            return new LevelObject();
        }

        protected virtual Apple CreateAppleObject()
        {
            return new Apple();
        }

        protected virtual void ReadLevelTag(Level creationLevel, XmlReader reader)
        {
            reader.MoveToAttribute("name");
            creationLevel.Name = reader.Value;

            reader.MoveToAttribute("width");
            creationLevel.LevelWidth = Int32.Parse(reader.Value);

            reader.MoveToAttribute("height");
            creationLevel.LevelHeight = Int32.Parse(reader.Value);

            reader.MoveToAttribute("targetScore");
            creationLevel.TargetScore = Int32.Parse(reader.Value);

            reader.MoveToAttribute("startSpeed");
            creationLevel.SnakeSpeed = Int32.Parse(reader.Value);
        }

        protected virtual void ReadSnakeTag(Level creationLevel, XmlReader reader)
        {
            reader.MoveToAttribute("snakeStartPosX");
            creationLevel.SnakeStartPosX = Int32.Parse(reader.Value);

            reader.MoveToAttribute("snakeStartPosY");
            creationLevel.SnakeStartPosY = Int32.Parse(reader.Value);
        }

        protected virtual void ReadAppleTag(Level creationLevel, XmlReader reader, Apple apple)
        {
            reader.MoveToAttribute("points");
            apple.Points = Int32.Parse(reader.Value);

            reader.MoveToAttribute("tailgrow");
            apple.TailGrow = Int32.Parse(reader.Value);

            reader.MoveToAttribute("special");
            bool isSpecial = Boolean.Parse(reader.Value);

            if (isSpecial)
            {
                AppleSpeciality special = new AppleSpeciality();
                
                reader.MoveToAttribute("type");
                special.Type = reader.Value;
                
                reader.MoveToAttribute("value");
                special.Value = Int32.Parse(reader.Value);

                reader.MoveToAttribute("prob");
                special.Probability = Int32.Parse(reader.Value);

                reader.MoveToAttribute("time");
                special.Time = Int32.Parse(reader.Value);

                apple.Speciality = special;
            }
        }

        protected virtual void ReadObjectTag(Level creationLevel, XmlReader reader, LevelObject levObject)
        {

            reader.MoveToAttribute("number");
            levObject.Number = Int32.Parse(reader.Value);

            reader.MoveToAttribute("posX");
            levObject.ObjectPosX = Int32.Parse(reader.Value);

            reader.MoveToAttribute("posY");
            levObject.ObjectPosY = Int32.Parse(reader.Value);

            reader.MoveToAttribute("sizeX");
            levObject.ObjectSizeX = Int32.Parse(reader.Value);

            reader.MoveToAttribute("sizeY");
            levObject.ObjectSizeY = Int32.Parse(reader.Value);

            reader.MoveToAttribute("name");
            levObject.Name = reader.Value;

            for (int x = levObject.ObjectPosX; x < levObject.ObjectPosX + levObject.ObjectSizeX; x++)
                for (int y = levObject.ObjectPosY; y < levObject.ObjectPosY + levObject.ObjectSizeY; y++)
                    creationLevel.Objects[x, y] = true;
        }
    }
}
