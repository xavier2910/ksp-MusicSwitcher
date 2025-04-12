using System;
using System.Collections.Generic;
using UnityEngine;
using Log = KSPBuildTools.Log;

namespace MusicSwitcher {

    [KSPAddon(KSPAddon.Startup.Flight, false)]
    internal class FlightMusic : MonoBehaviour {

        private readonly string logTag = "[FlightMusic]";

        #region UnityMessages

        private void Awake() {
            Statics.switcherInstance.ClearAll();
            Statics.switcherInstance.TakeOver();
        }

        private void Start() {
            LoadConfigs();
        }

        private void OnDestroy() {
            Statics.switcherInstance.ClearAll();
        }

        #endregion
        #region PrivateHelpers

        private void LoadConfigs() {
            int foundcfgs = 0;
            int loadedcfgs = 0;

            foreach (var url in GameDatabase.Instance.root.AllConfigs) {

                if (url.type != Statics.kFlightCFGType) {
                    continue;
                }

                foundcfgs++;

                ConfigNode node = url.config;
                var cfg = ConfigNode.CreateObjectFromConfig<FlightConfig>(node);

                
                Log.Debug($"Loading clip for '{cfg.debugName}' @ {cfg.trackPath}", logTag);
                AudioClip clip = LoadClip(cfg.trackPath);
                if (clip == null) {
                    Log.Warning($"Could not load clip for '{cfg.debugName}' @ path {cfg.trackPath}!", logTag);
                    continue;
                }
                loadedcfgs++;

                //EnsureTriggerExists(cfg.trigger);

                
            }

            Log.Debug($"loaded {loadedcfgs}/{foundcfgs} flight configs", logTag);
        }

        private AudioClip LoadClip(string gdb) {
            if (GameDatabase.Instance.ExistsAudioClip(gdb)) {
                return GameDatabase.Instance.GetAudioClip(gdb);
            } else {
                return null;
            }
        }


        #endregion

    }
}