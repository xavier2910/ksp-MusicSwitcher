using System;
using System.Collections.Generic;
using UnityEngine;

namespace MusicSwitcher.Behaviors
{
    [KSPAddon(KSPAddon.Startup.Credits, false)]
    internal class CreditsStockMusic : MonoBehaviour
    {
        private void Awake()
        {

            Statics.switcherInstance.ClearAll();
            Statics.switcherInstance.RevertToStock();
        }

        private void OnDestroy()
        {
            Statics.switcherInstance.ClearAll();
            Statics.switcherInstance.TakeOver();
        }
    }
}