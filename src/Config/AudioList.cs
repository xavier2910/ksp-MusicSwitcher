using System;
using System.Collections.Generic;
using UnityEngine;
using Log = KSPBuildTools.Log;

namespace MusicSwitcher.Config
{
    internal class AudioList
    {
        [Persistent] public string debugName = invalidPattern;
        [Persistent] public List<string> trackPaths = null;

        public static readonly string invalidPattern = "!INVALID!";

        private readonly string logTag = "[Config][AudioList]";

        public IEnumerable<AudioClip> Load() {

            foreach (var gdbPath in trackPaths)
            {
                if (GameDatabase.Instance.ExistsAudioClip(gdbPath)) {
                    Log.Debug($"Loaded clip @ {gdbPath}", logTag);
                    yield return GameDatabase.Instance.GetAudioClip(gdbPath);
                } else {
                    Log.Warning($"Could not load clip @ {gdbPath}!", logTag);
                }
            }
        }
    }
}