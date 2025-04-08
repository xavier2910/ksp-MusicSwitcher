using System;
using System.Collections.Generic;
using UnityEngine;

namespace MusicSwitcher
{
    internal class Settings
    {
        [Persistent] public float volumeMaster = 0.5f;

        public override string ToString() {
            return "MusicSwitcher.Settings{" + ConfigNode.CreateConfigFromObject(this).ToString().Replace('\n', '\t') + "}";
        }
    }
}