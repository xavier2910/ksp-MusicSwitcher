using System;
using System.Collections.Generic;

namespace MusicSwitcher
{
    internal class MusicControllerConfig
    {
        /// <summary>
        /// relative paths to audio files from within GameData. Do not include
        /// the file extension!
        /// </summary>
        [Persistent] public List<string> trackPaths = new List<string>();
        [Persistent] public string debugName = invalidPattern;
        [Persistent] public string typeName = invalidPattern;

        public static readonly string invalidPattern = "!invalid!";
    }
}