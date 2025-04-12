using System;
using System.Collections.Generic;
using UnityEngine;

namespace MusicSwitcher
{
    /// <summary>
    /// The job of a MusicController is to be a scene-specific, feature-specific music manager. Eg, you might have
    /// one designed to do background music based on location. You might have one do reentry riffs.
    /// You might have one lament your abysmal failures. On could add drama to the achievement of an escape
    /// trajectory. The edge of double-precision's the limit.
    /// </summary>
    internal interface MusicController
    {
        /// <summary>
        /// Regular tasks. I mean, use `GameEvent`s whenever possible, but when not, do what you must.
        /// </summary>
        void Update();

        void Add(AudioClip c);
        
        void Pause();
        void UnPause();

        /// <summary>
        /// Called on scene changes and the like; all operations on a `MusicController`
        /// should be noops after `Close()` has been called.
        /// </summary>
        void Close();
    }
}