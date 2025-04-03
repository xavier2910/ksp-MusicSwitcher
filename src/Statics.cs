using System;
using System.Collections.Generic;
using UnityEngine;

namespace MusicSwitcher
{
    /// <summary>
    /// Generally speaking, a class full of globals. In theory, these should be constant through the lifetime of
    /// the plugin.
    /// </summary>
    public sealed class Statics
    {
        /// <summary>
        /// This is the preferred way to access the global MusicSwitcher. It will be set by the MusicSwitcher
        /// on Awake().
        /// </summary>
        public static MusicSwitcher switcherInstance {get; internal set;}

        public static readonly string kSettingsCFGType = "MUSICSWITCHER_SETTINGS";

        public static class Clips
        {
            public static AudioClip mainMenuTheme {get; internal set;}
            public static AudioClip mainMenuAmbience {get; internal set;}
            public static AudioClip kscDay {get; internal set;}
            public static AudioClip kscNight {get; internal set;}
            public static AudioClip trackingStation {get; internal set;}
            public static AudioClip rnd {get; internal set;}
            public static AudioClip missionctl {get; internal set;}
            public static AudioClip admin {get; internal set;}
            public static AudioClip astroComplex {get; internal set;}
            public static AudioClip sph {get; internal set;}
            public static AudioClip vab {get; internal set;}

            public static AudioClip stratejazz {get => admin;}
        }

        internal static Settings globalSettings = new Settings();
    }
}