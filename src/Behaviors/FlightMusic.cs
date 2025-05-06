using System;
using System.Collections.Generic;
using UnityEngine;
using Log = KSPBuildTools.Log;

namespace MusicSwitcher.Behaviors {

    [KSPAddon(KSPAddon.Startup.Flight, false)]
    internal class FlightMusic : MonoBehaviour {

        private readonly string logTag = "[FlightMusic]";

        #region UnityMessages

        private void Awake()
        {
            Statics.switcherInstance.ClearAll();
            Statics.switcherInstance.TakeOver();
        }

        private void Start()
        {
            LoadConfigs();
        }

        private void OnDestroy()
        {
            Statics.switcherInstance.ClearAll();
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
                var cfg = ConfigNode.CreateObjectFromConfig<Config.Controller>(node);

                IController created = NewMusicController(node, cfg);
                if (created == null) {
                    Log.Warning($"failed to load config for {cfg.debugName}!", logTag);
                    continue;
                }
                Statics.switcherInstance.Register(created);

                loadedcfgs++;                
            }

            Log.Message($"loaded {loadedcfgs}/{foundcfgs} flight configs", logTag);
        }

        private IController NewMusicController(ConfigNode node, Config.Controller cfg) {

            IController mc = null;
            
            try {
                mc = Type.GetType(cfg.typeName)
                         .GetConstructor(new Type[] {})
                         .Invoke(new object[] {})
                      as IController;
            } catch (Exception e) {
                Log.Error($"for {cfg.debugName}: type {cfg.typeName} is inaccessible!", logTag);
                Log.Debug($"error: {e}", logTag);
                return null;
            }
            if (mc == null) {
                Log.Error($"for {cfg.debugName}: type {cfg.typeName} is not a valid MusicController!", logTag);
                Log.Message($"please ensure that {cfg.typeName} implements the MusicController interface", logTag);
                return mc;
            }


            mc.Initialize(Statics.switcherInstance.SourceWrangler, node);

            return mc;

        }

        #endregion

    }
}