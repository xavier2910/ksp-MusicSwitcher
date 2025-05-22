using System;
using System.Collections.Generic;
using MusicSwitcher.Util;
using UnityEngine;
using Log = KSPBuildTools.Log;

namespace MusicSwitcher.Controller {

    public abstract class Fades : IController {

        private readonly CoroutineManager routines = new();
        private State currentState;
        private float volume = 1f;
        private float fadeoutDelta = .05f;
        private readonly float pauseFadeDelta = .1f;

        protected string DebugName { get; private set; }

        #region Templates

        protected abstract string LogTag { get; }
        protected abstract bool ActivationCriterion { get; }
        protected abstract void InitializeTasks(AudioSourceWrangler asw, ConfigNode cfg);
        protected abstract void CloseTasks();
        protected abstract void ActivateTasks();
        protected abstract void WhileActive();
        protected abstract void SetVolume(float vol);
        protected abstract void Stop();
        protected abstract void Pause();
        protected abstract void UnPause();
        protected abstract void Play();

        #endregion
        #region IController
        public void Initialize(AudioSourceWrangler asw, ConfigNode cfg) {

            LoadDebugName(cfg);

            LoadAudioParams(cfg);

            BindEvents();

            InitializeTasks(asw, cfg);

            // do this last so ActivationCriterion() can be initialized
            SMInit();
        }

        private void LoadDebugName(ConfigNode cfg) {
            DebugName = Config.Util.DebugName(cfg);
        }

        private void LoadAudioParams(ConfigNode node) {
            volume = Config.Util.FloatOrDefault(
                "volume", node, 1f,
                $"{LogTag} for cfg '{DebugName}':");

            fadeoutDelta = Config.Util.FloatOrDefault(
                "fadeoutDelta", node, .05f,
                $"{LogTag} for cfg '{DebugName}':");
        }

        public void Update() {
            UpdateState();
            DispatchState();

            UpdateRoutines();
        }

        public void Close() {
            if (currentState == State.CLOSED) {
                return;
            }
            currentState = State.CLOSED;

            ReleaseEvents();
            CloseTasks();
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
                    if (!ActivationCriterion) {
                        Deactivate();
                    }
                    break;
                case State.INACTIVE:
                    if (ActivationCriterion) {
                        Activate();
                    }
                    break;
            }
        }

        private void Activate() {
            currentState = State.ACTIVE;
            ActivateTasks();
            volume = 1f;
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
                    WhileActive();
                    break;
                case State.INACTIVE:
                    break;
            }
        }

        private void SMInit() {
            if (ActivationCriterion) {
                Activate();
            } else {
                currentState = State.INACTIVE;
            }
        }

        private void SMPause() {
            if (currentState == State.ACTIVE || currentState == State.UNPAUSING) {
                routines.Add(PauseFade(pauseFadeDelta));
                currentState = State.PAUSING;
            }
        }

        private void SMUnPause() {
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
                Log.Debug($"set volume to {vol * 100}%", LogTag);
                yield return CoroutineState.RUNNING;
            }
            Stop();
            Log.Debug("stopping audio source", LogTag);
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
                Log.Debug($"set volume to {vol * 100}%", LogTag);
                yield return CoroutineState.RUNNING;
            }
            Pause();
            Log.Debug("pausing audio source", LogTag);
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
            UnPause();
            Log.Debug("unpausing audio source", LogTag);

            for (float vol = 0f; vol < 1f; vol += delta) {
                SetVolume(vol * this.volume);
                Log.Debug($"set volume to {vol * 100}%", LogTag);
                yield return CoroutineState.RUNNING;
            }

            SetVolume(volume);
            Log.Debug($"set volume to 100%", LogTag);
            currentState = State.ACTIVE;
            yield return CoroutineState.FINISHED;
        }

        #endregion
        #region Events

        private void BindEvents() {
            GameEvents.onGamePause.Add(SMPause);
            GameEvents.onGameUnpause.Add(SMUnPause);
        }

        private void ReleaseEvents() {
            GameEvents.onGamePause.Remove(SMPause);
            GameEvents.onGameUnpause.Remove(SMUnPause);
        }

        #endregion
    }
}