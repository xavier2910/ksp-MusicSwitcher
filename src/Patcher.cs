using System;
using System.Collections.Generic;
using UnityEngine;
using Log = KSPBuildTools.Log;

namespace MusicSwitcher
{

    [KSPAddon(KSPAddon.Startup.Instantly, true)]    
    public class Patcher : MonoBehaviour {
        
        private void Start() {
            var musicLogic = GameObject.Find("GameData/MusicLogic");

            if (musicLogic == null)
            {
                Log.Error("Could not find MusicLogic GameObject!", "[Patcher]");
            } else
            {
                Log.Message("Patching MusicLogic...", "[Patcher]");
            }

            musicLogic.AddComponent<MusicSwitcher>();
        }
    }
}