using System;
using System.Collections.Generic;
using UnityEngine;
using Log = KSPBuildTools.Log;

namespace MusicSwitcher.Controllers {

    internal class SituationalShuffle : IController {

        private AudioSourceWrangler w;
        private AudioSource src;

        private readonly List<AudioClip> tracks;
        private int currentTrack = int.MaxValue; // set to max int to induce a shuffle immediately on play start.
        private Vessel.Situations TargetSituation {get; set;}
        private Vessel.Situations CurrentSituation {get => FlightGlobals.ActiveVessel.situation;}
        private bool paused = true;

        private bool closed = false;

        private readonly System.Random rnd;
        private readonly string logTag = "[SituationalShuffle]";

        public SituationalShuffle() {
            rnd = new System.Random();
            tracks = new List<AudioClip>();
            _ = logTag;
        }

        public void Initialize(AudioSourceWrangler w, ConfigNode node) {
            this.w = w;
            src = w.Get();
            BindEvents();

            var audioCfg = ConfigNode.CreateObjectFromConfig<Config.AudioList>(node);
            foreach (var track in audioCfg.Load()) {
                Add(track);
            }
            if (tracks.Count == 0) {
                Log.Warning($"Controller '{audioCfg.debugName}' has no associated tracks!", logTag);
            }

            TargetSituation = ConfigNode.CreateObjectFromConfig<Config.Situation>(node).situation;
        }

        public void Add(AudioClip c) => tracks.Add(c);

        public void Pause() {
            src.Pause();
            paused = true;
        }
        public void UnPause() {
            if (TargetSituation == CurrentSituation) {
                src.UnPause();
                paused = false;
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
            if (tracks.Count == 0) {
                return;
            }
            if (!paused && CurrentSituation != TargetSituation) {
                Pause();
            } else if (paused && CurrentSituation == TargetSituation) {
                UnPause();
                currentTrack = int.MaxValue;
            }
            if (paused) {
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

        #region Events

        private void BindEvents() {
            GameEvents.onGamePause.Add(OnGamePause);
            GameEvents.onGameUnpause.Add(OnGameUnPause);
        }

        private void ReleaseEvents() {
            GameEvents.onGamePause.Remove(OnGamePause);
            GameEvents.onGameUnpause.Remove(OnGameUnPause);
        }

        private void OnGamePause() => Pause();
        private void OnGameUnPause() => UnPause();

        #endregion
    }
}