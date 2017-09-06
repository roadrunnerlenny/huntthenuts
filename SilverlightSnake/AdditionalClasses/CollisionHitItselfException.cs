using System;

namespace SilverlightSnake
{
    public class CollisionHitItselfException : CollisionException
    {
        public override string ToString()
        {
            return "Nicht in dich selbst laufen!";
        }
    }
}
