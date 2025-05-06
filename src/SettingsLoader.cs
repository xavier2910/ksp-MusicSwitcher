using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Log = KSPBuildTools.Log;

namespace MusicSwitcher
{

    [KSPAddon(KSPAddon.Startup.Instantly, true)]    
    public class SettingsLoader : MonoBehaviour {

        private string settingsPath;
        
        public void Awake() {

            bool settingsFileExists = true;
            settingsPath = AssemblyLoader.loadedAssemblies.GetPathByType(typeof(MusicSwitcher)) + "/";
            Log.Debug("settingsPath=\""+ settingsPath + "\"", "[SettingsLoader]");

            if (!Directory.Exists(settingsPath)) {
                Log.Message("settings path does not exist!", "[SettingsLoader]");
                Directory.CreateDirectory(settingsPath);
                settingsFileExists = false;
            }

            var node = ConfigNode.Load(settingsPath + Statics.kGlobalSettingsFile);
            settingsFileExists &= (node != null);

            bool needsNewSettingsFile = !settingsFileExists;

            if (settingsFileExists) {
                var settings = ConfigNode.CreateObjectFromConfig<Settings>(node.GetNode(Statics.kSettingsCFGType));
                if (settings != null) {
                    Statics.globalSettings = settings;
                    Log.Debug("Loaded settings: " + settings.ToString(), "[SettingsLoader]");
                } else {
                    needsNewSettingsFile = true;
                }
            }

            if (needsNewSettingsFile) {
                Log.Message("settings file " + settingsPath + Statics.kGlobalSettingsFile + " is corrupt or unreadable."
                    + " Overwriting with default settings....", "[SettingsLoader]");
                PersistDefaults();
            }
        }

        private void PersistDefaults() {
            var values = ConfigNode.CreateConfigFromObject(Statics.globalSettings);
            var settingsRoot = new ConfigNode();
            var settingsNode = settingsRoot.AddNode(Statics.kSettingsCFGType);
            values.CopyTo(settingsNode);

            settingsRoot.Save(settingsPath + Statics.kGlobalSettingsFile);
        }
    }
}