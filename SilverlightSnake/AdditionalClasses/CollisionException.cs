using System;

namespace SilverlightSnake
{
    public class CollisionException : Exception
    {
        public override string ToString()
        {
            return "In eine Wand oder Dich selbst gelaufen...";
        }
    }
}
