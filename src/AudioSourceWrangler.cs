using System;
using System.Collections.Generic;
using UnityEngine;
using Log = KSPBuildTools.Log;

namespace MusicSwitcher {

    public class AudioSourceWrangler {

        private List<AudioSource> inUse;
        private Stack<AudioSource> free;
        private GameObject host;
        private readonly string logTag = "[AudioSourceWrangler]";

        public AudioSourceWrangler(GameObject host) {
            this.host = host;
            inUse = new List<AudioSource>();
            free = new Stack<AudioSource>();
        }

        public AudioSource Get() {
            AudioSource gotten;

            try {
                gotten = free.Pop();
            } catch (InvalidOperationException) {
                gotten = NewSource();
            }
            inUse.Add(gotten);

            return gotten;
        }

        public void Release(AudioSource freed) {

            bool ok = inUse.Remove(freed);
            if (!ok) {
                Log.Debug($"`Release()` called on unrecognized AudioSource {freed}. It will now be tracked.", logTag);
            }

            free.Push(freed);
        }

        public void ReleaseAll() {
            foreach (AudioSource src in inUse) {
                Release(src);
            }
        }

        private AudioSource NewSource() {
            var newSource = host.AddComponent<AudioSource>();
            newSource.spatialBlend = 0.0f;
            return newSource;
        }
    }
}