using System;
using System.Collections.Generic;
using UnityEngine;

namespace MusicSwitcher.Behaviors
{
    [KSPAddon(KSPAddon.Startup.MainMenu, false)]
    internal class MainMenuStockMusic : MonoBehaviour
    {
        private static bool isFirstTime = true;

        private void Start()
        {

            var ms = Statics.switcherInstance;
            ms.ClearAll();
            ms.TakeOver();

            if (isFirstTime)
            {
                isFirstTime = false;
                ms.Enqueue(new Track(Statics.Clips.mainMenuTheme, false));
            }

            ms.Enqueue(new Track(Statics.Clips.mainMenuAmbience, true));
        }

        // we don't do any OnDestroy here so that the music continues into the settings scene.
        // all other scenes will take over with their own plugin.
        // unfortunately, this gets reloaded when you return from the settings scene to the
        // main menu, causing a divergence from stock behavior and a potentially jarring transition
        // from the fanfare to the ambience...
    }
}