using System;
using System.Collections.Generic;
using MusicSwitcher.Util;
using UnityEngine;
using Log = KSPBuildTools.Log;

namespace MusicSwitcher.Controller {

    internal sealed class SituationalShuffle : Fades {

        private AudioSourceWrangler w;
        private AudioSource src;

        private readonly List<AudioClip> tracks = new();
        private int currentTrack;
        private Vessel.Situations TargetSituation {get; set;}
        private Vessel.Situations CurrentSituation {get => FlightGlobals.ActiveVessel.situation;}

        private readonly System.Random rnd = new();
        private readonly string logTag = "[SituationalShuffle]";

        #region Fades

        protected override string LogTag => logTag;

        protected override bool ActivationCriterion
            => TargetSituation == CurrentSituation
            && tracks.Count > 0; // don't want no crashes eh?

        protected override void InitializeTasks(AudioSourceWrangler w, ConfigNode node) {
            this.w = w;
            src = w.Get();

            var audioCfg = ConfigNode.CreateObjectFromConfig<Config.AudioList>(node);
            foreach (var track in audioCfg.Load()) {
                Add(track);
            }
            if (tracks.Count == 0) {
                Log.Error($"Controller '{DebugName}' has no associated tracks!", logTag);
                return;
            }

            TargetSituation = ConfigNode.CreateObjectFromConfig<Config.Situation>(node).situation;

        }

        protected override void CloseTasks() {

            w.Release(src);
            src.Stop();
            src = null;
            tracks.Clear();
        }

        protected override void ActivateTasks() {
            currentTrack = int.MaxValue;
        }

        protected override void WhileActive() {
            if (ReadyForNextTrack()) {
                if (currentTrack >= tracks.Count) {
                    ShuffleTracks();
                    currentTrack = 0;
                }
                src.clip = tracks[currentTrack++];
                src.Play();
            }
        }

        protected override void SetVolume(float vol) {
            src.volume = vol * Statics.globalSettings.volumeMaster;
        }

        protected override void Stop() => src.Stop();

        protected override void Pause() => src.Pause();

        protected override void UnPause() => src.UnPause();

        protected override void Play() => src.Play();

        #endregion

        public void Add(AudioClip c) => tracks.Add(c);

        private bool ReadyForNextTrack() {
            return !src.isPlaying;
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