using System;

namespace SilverlightSnake
{
    public class CollisionHitObjectException : CollisionException
    {
        public override string ToString()
        {
            return "Nicht die Pflanzen berühren!";
        }
    }
}
