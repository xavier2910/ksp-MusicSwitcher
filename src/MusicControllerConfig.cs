using System;
using System.Collections.Generic;

namespace MusicSwitcher
{
    internal class MusicControllerConfig
    {
        [Persistent] public string debugName = invalidPattern;
        [Persistent] public string typeName = invalidPattern;

        public static readonly string invalidPattern = "!INVALID!";
    }
}