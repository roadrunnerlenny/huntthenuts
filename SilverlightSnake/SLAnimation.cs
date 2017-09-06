using System;

namespace SilverlightSnake
{
    public struct SLAnimation
    {
        public byte AnimationFrames { get; set; }
        public byte CollisionFrames { get; set; }
        public byte AnimationDelay { get; set; }
        public byte CollisionDelay { get; set; }
        public byte WinFrames { get; set; }
        public byte WinDelay { get; set; }
        public byte GrowFrames { get; set; }
        public byte GrowDelay { get; set; }
        public byte ShrinkFrames { get; set; }
        public byte ShrinkDelay { get; set; }
        public string FilenameTail { get; set; }
        public string FilenameCollision { get; set; }
        public string FilenameWin { get; set; }
        public string FilenameGrow { get; set; }
        public string FilenameShrink { get; set; }
    }
}
