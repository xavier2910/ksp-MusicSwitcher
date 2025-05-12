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

        private State currentState = State.INACTIVE;

        private readonly System.Random rnd;
        private readonly string logTag = "[SituationalShuffle]";

        public SituationalShuffle() {
            rnd = new System.Random();
            tracks = new List<AudioClip>();
            _ = logTag;
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
        }

        #endregion
        #region StateLogic

        private enum State {
            INACTIVE,            // ie not currently in the correct state and so not playing music
            ACTIVE,              // playing music
            PAUSED_WHILE_ACTIVE, // game paused
            CLOSED,              // this IController is no longer being used and should noop
                                 // until the garbage collector comes for it

            /* State Diagram:

                                                        +--game paused--> PAUSED_WHILE_ACTIVE  \
                                                        |                      |                \
                 +------Situation becomes Target---> ACTIVE <---game unpaused--+                 }--Close()  called--> CLOSED
                 |                                     |                                        /   or tracks empty
             INACTIVE <--Situation stops being target--+                                       /

                Note that we don't need to worry about pausing while CLOSED or INACTIVE, since those states
                produce no sound anyway.
            */
        }

        private void UpdateState() {
            switch (currentState) {
            case State.CLOSED:
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
        }

        private void Deactivate() {
            currentState = State.INACTIVE;
            src.Stop();
        }

        private void DispatchState() {
            switch (currentState) {
            case State.CLOSED:
                break;
            case State.PAUSED_WHILE_ACTIVE:
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

        private void Pause() {
            if (currentState == State.ACTIVE) {
                src.Pause();
                currentState = State.PAUSED_WHILE_ACTIVE;
            }
        }

        private void UnPause() {
            if (currentState == State.PAUSED_WHILE_ACTIVE) {
                src.UnPause();
                currentState = State.ACTIVE;
            }
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