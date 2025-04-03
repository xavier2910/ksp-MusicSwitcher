using System;
using System.Collections.Generic;
using UnityEngine;

namespace MusicSwitcher
{
    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    internal class EditorStockMusic : MonoBehaviour
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