using System;
using System.Collections.Generic;
using UnityEngine;
using Log = KSPBuildTools.Log;

namespace MusicSwitcher {

    internal class SachaReplacementCtlr : MusicController {

        private AudioSourceWrangler w;
        private AudioSource src;

        private List<AudioClip> tracks;
        private int currentTrack = 0;

        private bool closed = false;

        private System.Random rnd;

        public SachaReplacementCtlr(AudioSourceWrangler w) {
            this.w = w;
            src = w.Get();
            rnd = new System.Random();
            tracks = new List<AudioClip>();
        }

        public void Add(AudioClip c) => tracks.Add(c);

        public void Pause() => src.Pause();
        public void UnPause() => src.UnPause();

        public void Close() {
            if (closed) {
                return;
            }

            w.Release(src);
            src.Stop();
            src = null;
            tracks.Clear();
            closed = true;
        }

        public void Update() {
            if (closed) {
                return;
            }
            if (StillPlaying()) {
                return;
            }

            if (currentTrack < tracks.Count) {
                src.clip = tracks[currentTrack];
                src.Play();
                currentTrack++;
            } else {
                currentTrack = 0;
                ShuffleTracks();
                Update();
            }
        }

        private bool StillPlaying() {
            return src.isPlaying;
        }

        private void ShuffleTracks() {
            int j; AudioClip tmp;
            for (int i = 0; i < tracks.Count-1; i++ ) {
                j = rnd.Next(i, tracks.Count);
                tmp = tracks[i];
                tracks[i] = tracks[j];
                tracks[j] = tmp;
            }
        }
    }
}