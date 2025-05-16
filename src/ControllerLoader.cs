using System;
using System.Collections.Generic;
using UnityEngine;
using Log = KSPBuildTools.Log;

namespace MusicSwitcher {

    // start at PSystemSpawn so that any MainMenu music triggers properly,
    // but also so that the MusicSwitcher and GameDatabase have been properly
    // initialized before this starts up.
    [KSPAddon(KSPAddon.Startup.PSystemSpawn, false)]
    internal class ControllerLoader : MonoBehaviour {

        private readonly string logTag = "[ControllerLoader]";

        private readonly List<ConfigNode> flightCFGs = new();
        private readonly List<ConfigNode> editorCFGs = new();
        private readonly List<ConfigNode> mainMenuCFGs = new();
        private readonly List<ConfigNode> trackingStationCFGs = new();

        private GameScenes state;
        private GameScenes ActualState { get => HighLogic.LoadedScene; }

        #region UnityMessages

        private void Awake() {
            Statics.switcherInstance.ClearAll();
            Statics.switcherInstance.RevertToStock();
            DontDestroyOnLoad(this);
        }

        private void Start() {
            LoadConfigs();
        }

        private void LoadConfigs() {
            int foundcfgs = 0;
            foreach (var url in GameDatabase.Instance.root.AllConfigs) {

                if (url.type == $"{Statics.kControllerCFGType}{Statics.kFlightCFGTypeSuffix}") {
                    foundcfgs++;
                    flightCFGs.Add(url.config);
                } else if (url.type == $"{Statics.kControllerCFGType}{Statics.kEditorCFGTypeSuffix}") {
                    foundcfgs++;
                    editorCFGs.Add(url.config);
                } else if (url.type == $"{Statics.kControllerCFGType}{Statics.kMainMenuCFGTypeSuffix}") {
                    foundcfgs++;
                    mainMenuCFGs.Add(url.config);
                } else if (url.type == $"{Statics.kControllerCFGType}{Statics.kTrackingStationCFGTypeSuffix}") {
                    foundcfgs++;
                    trackingStationCFGs.Add(url.config);
                }

            }

            Log.Message($"found {foundcfgs} configs", logTag);
            Log.Message($"{flightCFGs.Count}/{foundcfgs} are flight configs", logTag);
            Log.Message($"{editorCFGs.Count}/{foundcfgs} are editor configs", logTag);
            Log.Message($"{mainMenuCFGs.Count}/{foundcfgs} are mainMenu configs", logTag);
            Log.Message($"{trackingStationCFGs.Count}/{foundcfgs} are trackingStation configs", logTag);

        }

        private void Update() {
            if (state == ActualState) {
                return;
            }

            state = ActualState;
            Statics.switcherInstance.ClearAll();
            Statics.switcherInstance.TakeOver();
            switch (state) {
                case GameScenes.FLIGHT:
                    // wait for flight mode to be properly initialized before starting
                    // set to PSYSTEM to provoke another state update next frame but
                    // not confuse the state machine by using something that presumably
                    // could happen.
                    if (FlightGlobals.ActiveVessel == null) {
                        state = GameScenes.PSYSTEM;
                        break;
                    }
                    {
                        int n = InitializeControllers(flightCFGs);
                        Log.Message($"initialized {n}/{flightCFGs.Count} flight configs", logTag);
                        if (n == 0) {
                            Statics.switcherInstance.RevertToStock();
                            Log.Message("no controllers initialized. reverting to stock music...");
                        }
                        break;
                    }
                case GameScenes.EDITOR:
                    {
                        int n = InitializeControllers(editorCFGs);
                        Log.Message($"initialized {n}/{editorCFGs.Count} editor configs", logTag);
                        if (n == 0) {
                            Statics.switcherInstance.RevertToStock();
                            Log.Message("no controllers initialized. reverting to stock music...");
                        }
                        break;
                    }
                case GameScenes.MAINMENU:
                    {
                        int n = InitializeControllers(mainMenuCFGs);
                        Log.Message($"initialized {n}/{mainMenuCFGs.Count} main menu configs", logTag);
                        if (n == 0) {
                            Statics.switcherInstance.RevertToStock();
                            Log.Message("no controllers initialized. reverting to stock music...");
                        }
                        break;
                    }
                case GameScenes.TRACKSTATION:
                    {
                        int n = InitializeControllers(trackingStationCFGs);
                        Log.Message($"initialized {n}/{trackingStationCFGs.Count} tracking station configs", logTag);
                        if (n == 0) {
                            Statics.switcherInstance.RevertToStock();
                            Log.Message("no controllers initialized. reverting to stock music...");
                        }
                        break;
                    }
                default:
                    Statics.switcherInstance.RevertToStock();
                    Log.Message("unsupported scene. reverting to stock music...");
                    break;

            }
        }

        private void OnDestroy() {
            Statics.switcherInstance.ClearAll();
        }

        #endregion
        #region Helpers

        private int InitializeControllers(List<ConfigNode> nodes) {

            int loadedCFGs = 0;

            foreach (var node in nodes) {

                var cfg = ConfigNode.CreateObjectFromConfig<Config.Controller>(node);
                Log.Debug($"initializing {cfg.debugName}...", logTag);

                IController created = NewMusicController(node, cfg);
                if (created == null) {
                    Log.Warning($"failed to load config for {cfg.debugName}!", logTag);
                    continue;
                }
                Statics.switcherInstance.Register(created);
                loadedCFGs++;
            }
            return loadedCFGs;
        }

        private IController NewMusicController(ConfigNode node, Config.Controller cfg) {

            IController mc;

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
                return null;
            }

            mc.Initialize(Statics.switcherInstance.SourceWrangler, node);

            return mc;
        }

        #endregion
    }
}