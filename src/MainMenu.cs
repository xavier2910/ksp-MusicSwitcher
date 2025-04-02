using System;
using System.Collections.Generic;
using UnityEngine;

namespace MusicSwitcher
{
    [KSPAddon(KSPAddon.Startup.MainMenu, false)]
    internal class MainMenu : MonoBehaviour
    {
        private static bool isFirstTime = true;

        private void Start() {

            var ms = Statics.GetSwitcher();
            ms.ClearAll();

            if (isFirstTime)
            {
                isFirstTime = false;
                ms.Enqueue(new Track(Statics.Clips.mainMenuTheme, false));
            }

            ms.Enqueue(new Track(Statics.Clips.mainMenuAmbience, true));
        }
    }
}