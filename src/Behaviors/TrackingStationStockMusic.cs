using System;
using System.Collections.Generic;
using UnityEngine;

namespace MusicSwitcher.Behaviors
{
    [KSPAddon(KSPAddon.Startup.TrackingStation, false)]
    internal class TrackingStationStockMusic : MonoBehaviour
    {
        private void Awake() {
            Statics.switcherInstance.ClearAll();
            Statics.switcherInstance.RevertToStock();
        }

        private void OnDestroy() {
            Statics.switcherInstance.ClearAll();
            Statics.switcherInstance.TakeOver();
        }
    }
}