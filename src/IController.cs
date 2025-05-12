using System;
using System.Collections.Generic;
using UnityEngine;

namespace MusicSwitcher
{
    /// <summary>
    /// The job of a music Controller is to be a scene-specific, feature-specific music manager. Eg, you might have
    /// one designed to do background music based on location. You might have one do reentry riffs.
    /// You might have one lament your abysmal failures. On could add drama to the achievement of an escape
    /// trajectory. The edge of double-precision's the limit.
    /// </summary>
    public interface IController
    {

        /// <summary>
        /// called by the loader when the Controller is loaded.
        /// </summary>
        void Initialize(AudioSourceWrangler asw, ConfigNode cfg);

        /// <summary>
        /// Regular tasks. I mean, use `GameEvent`s whenever possible, but when not, do what you must.
        /// </summary>
        void Update();

        /// <summary>
        /// Called on scene changes and the like; all operations on a `IController`
        /// should be noops after `Close()` has been called.
        /// </summary>
        void Close();
    }
}