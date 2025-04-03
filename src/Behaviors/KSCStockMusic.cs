using System;
using System.Collections.Generic;
using UnityEngine;

namespace MusicSwitcher
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    internal class KSCStockMusic : MonoBehaviour
    {
        private void Awake() {

            Statics.switcherInstance.ClearAll();
            Statics.switcherInstance.RevertToStock(true);
        }

        private void OnDestroy() {
            Statics.switcherInstance.ClearAll();
            Statics.switcherInstance.TakeOver();
        }
    }
}