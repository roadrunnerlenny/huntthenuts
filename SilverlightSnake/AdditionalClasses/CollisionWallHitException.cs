using System;

namespace SilverlightSnake
{
    public class CollisionWallHitException : CollisionException
    {
        public override string ToString()
        {
            return "Aua - in die Hecke gelaufen...";
        }
    }
}
