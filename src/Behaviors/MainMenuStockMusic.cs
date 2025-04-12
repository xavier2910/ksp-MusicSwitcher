using System;
using System.Collections.Generic;
using UnityEngine;

namespace MusicSwitcher
{
    [KSPAddon(KSPAddon.Startup.MainMenu, false)]
    internal class MainMenuStockMusic : MonoBehaviour
    {
        private void Start() {
            var ms = Statics.switcherInstance;
            ms.ClearAll();
            ms.RevertToStock();
        }

        private void OnDestroy() {
            Statics.switcherInstance.ClearAll();
        }
    }
}