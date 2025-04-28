using System;
using System.Collections.Generic;
using UnityEngine;
using Log = KSPBuildTools.Log;

namespace MusicSwitcher.Controllers {

    internal class SpaceShuffle : IController {

        private AudioSourceWrangler w;
        private AudioSource src;

        private List<AudioClip> tracks;
        private int currentTrack = 0;
        private bool inSpace = false;

        private bool closed = false;

        private System.Random rnd;

        public SpaceShuffle(AudioSourceWrangler w) {
            this.w = w;
            src = w.Get();
            rnd = new System.Random();
            tracks = new List<AudioClip>();

            BindEvents();
        }

        public void Add(AudioClip c) => tracks.Add(c);

        public void Pause() => src.Pause();
        public void UnPause() {
            if (inSpace) {
                src.UnPause();
            }
        }

        public void Close() {
            if (closed) {
                return;
            }

            w.Release(src);
            src.Stop();
            src = null;
            tracks.Clear();
            closed = true;
            ReleaseEvents();
        }

        public void Update() {
            if (closed) {
                return;
            }
            if (StillPlaying()) {
                return;
            }
            if (!inSpace) {
                Pause();
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

        #region Events

        private void BindEvents() {
            GameEvents.onGamePause.Add(OnGamePause);
            GameEvents.onGameUnpause.Add(OnGameUnPause);
            GameEvents.VesselSituation.onReachSpace.Add(OnEnterSpace);
        }

        private void ReleaseEvents() {
            GameEvents.onGamePause.Remove(OnGamePause);
            GameEvents.onGameUnpause.Remove(OnGameUnPause);
            GameEvents.VesselSituation.onReachSpace.Remove(OnEnterSpace);
        }

        private void OnEnterSpace(object o) {
            inSpace = true;
            UnPause();
        }
        private void OnGamePause() => Pause();
        private void OnGameUnPause() => UnPause();

        #endregion
    }
}