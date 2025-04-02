using System;
using System.Collections.Generic;
using UnityEngine;

namespace MusicSwitcher
{
    public sealed class Statics
    {
        /// <summary>
        /// This is the preferred way to access the global MusicSwitcher. This should
        /// not be called repeatedly; cache it.
        /// </summary>
        public static MusicSwitcher GetSwitcher()
        {
            return GameObject.FindWithTag(kMusicSwitcherTag).GetComponent<MusicSwitcher>();
        }

        internal static readonly string kMusicSwitcherTag = "MusicSwitcher";
        internal static readonly int kNLayersDefault = 3;

        internal static class Clips
        {
            public static AudioClip mainMenuTheme;
            public static AudioClip mainMenuAmbience;
        }
    }
}