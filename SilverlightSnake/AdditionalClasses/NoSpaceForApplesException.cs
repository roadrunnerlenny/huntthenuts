using System;

namespace SilverlightSnake
{
    public class NoSpaceForApplesException : Exception
    {
        public override string ToString()
        {
            return "Kein Platz mehr für weitere Nüsse!";
        }
    }
}
