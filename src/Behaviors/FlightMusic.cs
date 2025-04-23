using System;
using System.Collections.Generic;
using UnityEngine;
using Log = KSPBuildTools.Log;

namespace MusicSwitcher.Behaviors
{

    [KSPAddon(KSPAddon.Startup.Flight, false)]
    internal class FlightMusic : MonoBehaviour
    {

        private Dictionary<string /*condition*/, List<Track> /*to q*/> conditionalTracks
            = new Dictionary<string, List<Track>>();

        private readonly string logTag = "[FlightMusic]";
        private readonly int mainLayer = 0;

        #region UnityMessages

        private void Awake()
        {
            Statics.switcherInstance.ClearAll();
            Statics.switcherInstance.TakeOver();
        }

        private void Start()
        {
            LoadConfigs();
            BindEvents();
        }

        private void OnDestroy()
        {
            Statics.switcherInstance.ClearAll();
            ReleaseEvents();
        }

        #endregion
        #region GameEventHandlers

        private void BindEvents()
        {
            GameEvents.VesselSituation.onReachSpace.Add(OnEnterSpace);
        }

        private void ReleaseEvents()
        {
            GameEvents.VesselSituation.onReachSpace.Remove(OnEnterSpace);
        }

        private void OnEnterSpace(Vessel _)
        {
            QueueTrigger(nameof(OnEnterSpace));
        }

        #endregion
        #region PrivateHelpers

        private void LoadConfigs()
        {
            int foundcfgs = 0;
            int loadedcfgs = 0;

            foreach (var url in GameDatabase.Instance.root.AllConfigs)
            {

                if (url.type != Statics.kFlightCFGType)
                {
                    continue;
                }

                foundcfgs++;

                ConfigNode node = url.config;
                var cfg = ConfigNode.CreateObjectFromConfig<FlightConfig>(node);

                if (cfg.trigger == FlightConfig.invalidPattern)
                {
                    Log.Warning($"'{cfg.debugName}' has an empty or invalid 'trigger' field", logTag);
                    continue;
                }

                Log.Debug($"Loading clip for '{cfg.debugName}' @ {cfg.trackPath}", logTag);
                AudioClip clip = LoadClip(cfg.trackPath);
                if (clip == null)
                {
                    Log.Warning($"Could not load clip for '{cfg.debugName}' @ path {cfg.trackPath}!", logTag);
                    continue;
                }
                loadedcfgs++;

                EnsureTriggerExists(cfg.trigger);

                conditionalTracks[cfg.trigger].Add(
                    new Track(clip, cfg.looping)
                );
            }

            Log.Debug($"loaded {loadedcfgs}/{foundcfgs} flight configs", logTag);
        }

        private AudioClip LoadClip(string gdb)
        {
            if (GameDatabase.Instance.ExistsAudioClip(gdb))
            {
                return GameDatabase.Instance.GetAudioClip(gdb);
            }
            else
            {
                return null;
            }
        }

        private void EnsureTriggerExists(string cond)
        {
            try
            {
                _ = conditionalTracks[cond];
            }
            catch (KeyNotFoundException)
            {
                conditionalTracks[cond] = new List<Track>();
            }
        }

        private void QueueTrigger(string trigger)
        {
            List<Track> toQ;

            try
            {
                toQ = conditionalTracks[trigger];
            }
            catch (KeyNotFoundException)
            {
                Log.Debug($"trigger {trigger} has no subscribers", logTag);
                return;
            }
            Statics.switcherInstance.Clear(mainLayer);

            foreach (var track in toQ)
            {
                Statics.switcherInstance.Enqueue(track, mainLayer);
            }
        }

        #endregion

    }
}