using System;
using System.Collections.Generic;
using UnityEngine;

namespace MusicSwitcher.Config {
    
    internal class Situation {
        [Persistent] public string debugName = invalidPattern;
        [Persistent] public Vessel.Situations situation = Vessel.Situations.PRELAUNCH;

        public static readonly string invalidPattern = "!INVALID!";
    }
}