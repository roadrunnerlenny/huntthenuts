using System;

namespace SilverlightSnake
{
    public struct AppleSpeciality
    {
        public const string WinType = "win";
        public const string SpeedingType = "speeding";
        public const string ShrinkingType = "shrinking";
        public const string InverseType = "inverse";
        
        public string Type { get; set; }

        public int Value { get; set; }

        public int Time { get; set; }

        public int Probability { get; set; }
    }
}
