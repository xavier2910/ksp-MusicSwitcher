using System;
using System.Collections.Generic;
using UnityEngine;
using Log = KSPBuildTools.Log;

namespace MusicSwitcher.Util {
    internal class CoroutineManager {

        private List<IEnumerator<CoroutineState>> routines;

        public CoroutineManager() {
            routines = new List<IEnumerator<CoroutineState>>(0);
        }

        public void Add(IEnumerator<CoroutineState> routine) {
            routines.Add(routine);
        }

        public void Update() {

            foreach (var routine in routines) {
                var state = routine.Current;
                switch (state) {
                case CoroutineState.FINISHED:
                    routines.Remove(routine);
                    break;
                case CoroutineState.RUNNING:
                    routine.MoveNext();
                    break;
                }
            }
        }

    }

    internal enum CoroutineState {
        RUNNING,
        FINISHED,
    }

}