using System;
using System.Collections.Generic;
using UnityEngine;

namespace MusicSwitcher
{
    public sealed class Statics
    {
        /// <summary>
        /// This is the preferred way to access the global MusicSwitcher. It will be set by the MusicSwitcher
        /// on Awake().
        /// </summary>
        public static MusicSwitcher switcherInstance {get; internal set;}

        internal static readonly string kMusicSwitcherTag = "MusicSwitcher";
        internal static readonly int kNLayersDefault = 3;

        internal static class Clips
        {
            public static AudioClip mainMenuTheme;
            public static AudioClip mainMenuAmbience;
            public static AudioClip kscDay;
            public static AudioClip kscNight;
            public static AudioClip trackingStation;
            public static AudioClip rnd;
            public static AudioClip missionctl;
            public static AudioClip admin;
            public static AudioClip astroComplex;
            public static AudioClip sph;
            public static AudioClip vab;
        }
    }
}