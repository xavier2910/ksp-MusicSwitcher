using System;
using System.Collections.Generic;
using MusicSwitcher.Util;
using UnityEngine;
using Log = KSPBuildTools.Log;

namespace MusicSwitcher.Controllers {

    internal class SpaceShuffle : IController {

        private AudioSourceWrangler w;
        private AudioSource src;
        private readonly CoroutineManager coroutines;
        private float fadeoutDelta = .05f;
        private readonly float pauseFadeDelta = .1f;

        private readonly List<AudioClip> tracks;
        private int currentTrack = int.MaxValue; // set to max int to induce a shuffle immediately on play start.
        private State currentState = State.INACTIVE;
        private bool InSpace { get => FlightGlobals.ActiveVessel.orbit.referenceBody.bodyName != "Kerbin"
                                   || FlightGlobals.ActiveVessel.situation == Vessel.Situations.ORBITING
                                   || FlightGlobals.ActiveVessel.situation == Vessel.Situations.SUB_ORBITAL
                                   || FlightGlobals.ActiveVessel.situation == Vessel.Situations.ESCAPING
                                   || FlightGlobals.ActiveVessel.situation == Vessel.Situations.DOCKED;
        }

        private readonly System.Random rnd;
        private readonly string logTag = "[SpaceShuffle]";

        public SpaceShuffle() {
            rnd = new System.Random();
            tracks = new List<AudioClip>();
            _ = logTag;
            coroutines = new CoroutineManager();
        }

        #region IController
        public void Initialize(AudioSourceWrangler w, ConfigNode node) {
            this.w = w;
            src = w.Get();
            BindEvents();

            var cfg = ConfigNode.CreateObjectFromConfig<Config.AudioList>(node);
            foreach (var track in cfg.Load()) {
                Add(track);
            }
            if (tracks.Count == 0) {
                Log.Warning($"Controller '{cfg.debugName}' has no associated tracks!", logTag);
            }

            fadeoutDelta = Config.Util.FloatOrDefault(
                "fadeoutDelta", node, .05f,
                $"{logTag} for cfg '{cfg.debugName}':");

            if (InSpace) {
                currentState = State.ACTIVE;
            } else {
                currentState = State.INACTIVE;
            }
        }

        public void Add(AudioClip c) => tracks.Add(c);

        public void Close() {
            if (currentState == State.CLOSED) {
                return;
            }

            w.Release(src);
            src.Stop();
            src = null;
            tracks.Clear();
            currentState = State.CLOSED;
            ReleaseEvents();
        }

        public void Update() {
            UpdateState();
            DispatchState();

            UpdateCoroutines();
        }

        #endregion
        #region StateMachine

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
                 +--------Ship enters space--------> ACTIVE <---game unpaused--+                 }--Close()  called--> CLOSED
                 |                                     |         UNPAUSING                      /   or tracks empty
             INACTIVE <-------Ship leaves space--------+                                       /

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
                    if (!InSpace) {
                        Deactivate();
                    }
                    break;
                case State.INACTIVE:
                    if (InSpace) {
                        Activate();
                    }
                    break;
            }
        }

        private void Activate() {
            currentState = State.ACTIVE;
            currentTrack = int.MaxValue;
        }

        private void Deactivate() {
            currentState = State.INACTIVE;
            coroutines.Add(FadeOut(fadeoutDelta));
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

        public void Pause() {
            if (currentState == State.ACTIVE) {
                currentState = State.PAUSING;
                coroutines.Add(PauseFade(pauseFadeDelta));
            }
        }
        public void UnPause() {
            if (currentState == State.PAUSED_WHILE_ACTIVE) {
                currentState = State.UNPAUSING;
                coroutines.Add(UnPauseFade(pauseFadeDelta));
            }
        }

        #endregion
        #region Coroutines

        private void UpdateCoroutines() {
            coroutines.Update();
        }

        /// <summary>
        /// coroutine to fade out and stop the audio source
        /// </summary>
        /// <param name="delta">
        /// fraction per frame (out of 1)
        /// </param>
        private IEnumerator<CoroutineState> FadeOut(float delta) {

            for (float vol = 1f; vol > 0f; vol -= delta) {
                SetVolume(vol);
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
                SetVolume(vol);
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
                SetVolume(vol);
                Log.Debug($"set volume to {vol * 100}%", logTag);
                yield return CoroutineState.RUNNING;
            }

            SetVolume(1f);
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