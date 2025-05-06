using System;
using System.Collections.Generic;

namespace MusicSwitcher.Config
{
    internal class Controller
    {
        [Persistent] public string debugName = invalidPattern;
        [Persistent] public string typeName = invalidPattern;

        public static readonly string invalidPattern = "!INVALID!";
    }
}