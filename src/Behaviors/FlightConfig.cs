using System;

namespace MusicSwitcher
{
    internal class FlightConfig
    {
        /// <summary>
        /// relative path to audio file from within GameData. Do not include
        /// the file extension!
        /// </summary>
        [Persistent] public string trackPath = invalidPattern;
        [Persistent] public bool looping = false;
        [Persistent] public string trigger = invalidPattern;
        [Persistent] public string debugName = invalidPattern;

        public static readonly string invalidPattern = "invalid";
    }
}