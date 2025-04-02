using System;
using System.Collections.Generic;
using UnityEngine;

namespace MusicSwitcher
{

    [KSPAddon(KSPAddon.Startup.Instantly, true)]    
    public class Patcher : MonoBehaviour {
        
        private void Start() {
            var musicLogic = GameObject.Find("GameData/MusicLogic");

            if (musicLogic == null)
            {
                Debug.LogError("[MusicSwitcher.Patcher] Could not find MusicLogic GameObject!");
            } else
            {
                Debug.Log("[MusicSwitcher.Patcher] Patching MusicLogic...");
            }

            musicLogic.AddComponent<MusicSwitcher>();
        }
    }
}