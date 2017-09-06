using System;

namespace SilverlightSnake
{
    public class Apple
    {
        public int ApplePosX { get; set; }
        public int ApplePosY { get; set; }

        public int Points { get; set; }

        public int TailGrow { get; set; }

        public bool IsSet { get; set; }

        public AppleSpeciality Speciality { get; set; }
        
        public bool IsSpecial {
            get
            {
                if (this.Speciality.Type != null)
                    return true;
                else 
                    return false;
            }
        }
    }
}
