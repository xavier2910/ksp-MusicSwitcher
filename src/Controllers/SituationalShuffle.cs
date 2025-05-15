using System;
using System.Collections.Generic;
using MusicSwitcher.Util;
using UnityEngine;
using Log = KSPBuildTools.Log;

namespace MusicSwitcher.Controllers {

    internal class SituationalShuffle : IController {

        private AudioSourceWrangler w;
        private AudioSource src;

        private float volume = 1f;
        private float fadeoutDelta = .05f;
        private readonly float pauseFadeDelta = .1f;
        private readonly List<AudioClip> tracks;
        private int currentTrack = int.MaxValue; // set to max int to induce a shuffle immediately on play start.
        private Vessel.Situations TargetSituation {get; set;}
        private Vessel.Situations CurrentSituation {get => FlightGlobals.ActiveVessel.situation;}

        private State currentState = State.INACTIVE;

        private readonly CoroutineManager routines;

        private readonly System.Random rnd;
        private readonly string logTag = "[SituationalShuffle]";

        public SituationalShuffle() {
            rnd = new System.Random();
            tracks = new List<AudioClip>();
            _ = logTag;
            routines = new CoroutineManager();
        }

        #region IController

        public void Initialize(AudioSourceWrangler w, ConfigNode node) {
            this.w = w;
            src = w.Get();
            BindEvents();

            var audioCfg = ConfigNode.CreateObjectFromConfig<Config.AudioList>(node);
            foreach (var track in audioCfg.Load()) {
                Add(track);
            }
            if (tracks.Count == 0) {
                Log.Error($"Controller '{audioCfg.debugName}' has no associated tracks!", logTag);
                currentState = State.CLOSED;
                return;
            }

            TargetSituation = ConfigNode.CreateObjectFromConfig<Config.Situation>(node).situation;

            if (TargetSituation == CurrentSituation) {
                currentState = State.ACTIVE;
            }

            volume = Config.Util.FloatOrDefault(
                "volume", node, 1f,
                $"{logTag} for cfg '{audioCfg.debugName}':");

            fadeoutDelta = Config.Util.FloatOrDefault(
                "fadeoutDelta", node, .05f,
                $"{logTag} for cfg '{audioCfg.debugName}':");
        }

        public void Add(AudioClip c) => tracks.Add(c);

        public void Close() {
            if (currentState == State.CLOSED) {
                return;
            }

            currentState = State.CLOSED;

            w.Release(src);
            src.Stop();
            src = null;
            tracks.Clear();
            ReleaseEvents();
        }

        public void Update() {
            UpdateState();
            DispatchState();

            UpdateRoutines();
        }

        #endregion
        #region StateLogic

        private enum State {
            INACTIVE,            // ie not currently in the correct state and so not playing music
            ACTIVE,              // playing music
            PAUSED_WHILE_ACTIVE, // game paused
            PAUSING,
            UNPAUSING,
            CLOSED,              // this IController is no longer being used and should noop
                                 // until the garbage collector comes for it

            /* State Diagram:
                                                            PAUSING
                                                        +--game paused--> PAUSED_WHILE_ACTIVE  \
                                                        |                      |                \
                 +------Situation becomes Target---> ACTIVE <---game unpaused--+                 }--Close()  called--> CLOSED
                 |                                     |         UNPAUSING                      /   or tracks empty
             INACTIVE <--Situation stops being target--+                                       /

                Note that we don't need to worry about pausing while CLOSED or INACTIVE, since those states
                produce no sound anyway.
            */
        }

        private void UpdateState() {
            switch (currentState) {
                case State.CLOSED:
                    break;
                case State.PAUSING:
                    break;
                case State.UNPAUSING:
                    break;
                case State.PAUSED_WHILE_ACTIVE:
                    break;
                case State.ACTIVE:
                    if (TargetSituation != CurrentSituation) {
                        Deactivate();
                    }
                    break;
                case State.INACTIVE:
                    if (TargetSituation == CurrentSituation) {
                        Activate();
                    }
                    break;
            }
        }

        private void Activate() {
            currentState = State.ACTIVE;
            currentTrack = int.MaxValue;
            SetVolume(volume);
        }

        private void Deactivate() {
            currentState = State.INACTIVE;
            routines.Add(FadeOut(fadeoutDelta));
        }

        private void DispatchState() {
            switch (currentState) {
                case State.CLOSED:
                    break;
                case State.PAUSED_WHILE_ACTIVE:
                    break;
                case State.PAUSING:
                    break;
                case State.UNPAUSING:
                    break;
                case State.ACTIVE:
                    if (ReadyForNextTrack()) {
                        if (currentTrack >= tracks.Count) {
                            ShuffleTracks();
                            currentTrack = 0;
                        }
                        src.clip = tracks[currentTrack++];
                        src.Play();
                    }
                    break;
                case State.INACTIVE:
                    break;
            }
        }

        private bool ReadyForNextTrack() {
            return !src.isPlaying && currentState == State.ACTIVE;
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

        private void Pause() {
            if (currentState == State.ACTIVE || currentState == State.UNPAUSING) {
                routines.Add(PauseFade(pauseFadeDelta));
                currentState = State.PAUSING;
            }
        }

        private void UnPause() {
            if (currentState == State.PAUSED_WHILE_ACTIVE || currentState == State.PAUSING) {
                routines.Add(UnPauseFade(pauseFadeDelta));
                currentState = State.UNPAUSING;
            }
        }

        #endregion
        #region Routines

        private void UpdateRoutines() {
            routines.Update();
        }

        /// <summary>
        /// coroutine to fade out and stop the audio source
        /// </summary>
        /// <param name="delta">
        /// fraction per frame (out of 1)
        /// </param>
        private IEnumerator<CoroutineState> FadeOut(float delta) {

            for (float vol = 1f; vol > 0f; vol -= delta) {
                SetVolume(vol * this.volume);
                Log.Debug($"set volume to {vol * 100}%", logTag);
                yield return CoroutineState.RUNNING;
            }
            src.Stop();
            Log.Debug("stopping audio source", logTag);
            yield return CoroutineState.FINISHED;
        }

        /// <summary>
        /// coroutine to fade out and pause the audio source
        /// </summary>
        /// <param name="delta">
        /// fraction per frame (out of 1)
        /// </param>
        private IEnumerator<CoroutineState> PauseFade(float delta) {

            for (float vol = 1f; vol > 0f; vol -= delta) {
                SetVolume(vol * this.volume);
                Log.Debug($"set volume to {vol * 100}%", logTag);
                yield return CoroutineState.RUNNING;
            }
            src.Pause();
            Log.Debug("pausing audio source", logTag);
            currentState = State.PAUSED_WHILE_ACTIVE;
            yield return CoroutineState.FINISHED;
        }

        /// <summary>
        /// coroutine to unpause and fade in the audio source
        /// </summary>
        /// <param name="delta">
        /// fraction per frame (out of 1)
        /// </param>
        private IEnumerator<CoroutineState> UnPauseFade(float delta) {
            SetVolume(0f);
            src.UnPause();
            Log.Debug("unpausing audio source", logTag);

            for (float vol = 0f; vol < 1f; vol += delta) {
                SetVolume(vol * this.volume);
                Log.Debug($"set volume to {vol * 100}%", logTag);
                yield return CoroutineState.RUNNING;
            }

            SetVolume(volume);
            Log.Debug($"set volume to 100%", logTag);
            currentState = State.ACTIVE;
            yield return CoroutineState.FINISHED;
        }

        private void SetVolume(float vol) {
            src.volume = vol * Statics.globalSettings.volumeMaster;
        }

        #endregion
        #region Events

        private void BindEvents() {
            GameEvents.onGamePause.Add(Pause);
            GameEvents.onGameUnpause.Add(UnPause);
        }

        private void ReleaseEvents() {
            GameEvents.onGamePause.Remove(Pause);
            GameEvents.onGameUnpause.Remove(UnPause);
        }

        #endregion

    }
}