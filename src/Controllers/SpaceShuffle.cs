using System;
using System.Collections.Generic;
using UnityEngine;
using Log = KSPBuildTools.Log;

namespace MusicSwitcher.Controllers {

    internal class SpaceShuffle : IController {

        private AudioSourceWrangler w;
        private AudioSource src;

        private readonly List<AudioClip> tracks;
        private int currentTrack = int.MaxValue; // set to max int to induce a shuffle immediately on play start.
        private bool InSpace { get => FlightGlobals.ActiveVessel.orbit.referenceBody.bodyName != "Kerbin"
                                   || FlightGlobals.ActiveVessel.situation == Vessel.Situations.ORBITING
                                   || FlightGlobals.ActiveVessel.situation == Vessel.Situations.SUB_ORBITAL
                                   || FlightGlobals.ActiveVessel.situation == Vessel.Situations.ESCAPING
                                   || FlightGlobals.ActiveVessel.situation == Vessel.Situations.DOCKED;
        }
        private bool paused = false;

        private bool closed = false;

        private readonly System.Random rnd;
        private readonly string logTag = "[SpaceShuffle]";

        public SpaceShuffle() {
            rnd = new System.Random();
            tracks = new List<AudioClip>();
        }

        public void Initialize(AudioSourceWrangler w, ConfigNode node) {
            this.w = w;
            src = w.Get();
            BindEvents();

            var cfg = ConfigNode.CreateObjectFromConfig<Config.AudioList>(node);
            foreach (var track in cfg.Load()) {
                Add(track);
            }
        }

        public void Add(AudioClip c) => tracks.Add(c);

        public void Pause() {
            src.Pause();
            paused = true;
        }
        public void UnPause() {
            if (InSpace) {
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
            if (!InSpace) {
                Pause();
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
            GameEvents.VesselSituation.onReachSpace.Add(OnEnterSpace);
        }

        private void ReleaseEvents() {
            GameEvents.onGamePause.Remove(OnGamePause);
            GameEvents.onGameUnpause.Remove(OnGameUnPause);
            GameEvents.VesselSituation.onReachSpace.Remove(OnEnterSpace);
        }

        private void OnEnterSpace(object o) {
            UnPause();
        }
        private void OnGamePause() => Pause();
        private void OnGameUnPause() => UnPause();

        #endregion
    }
}