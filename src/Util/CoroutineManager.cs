using System.Collections.Generic;

namespace MusicSwitcher.Util {
    internal class CoroutineManager {

        private List<IEnumerator<CoroutineState>> routines;
        private Stack<IEnumerator<CoroutineState>> toRemove;

        public CoroutineManager() {
            routines = new List<IEnumerator<CoroutineState>>();
            toRemove = new Stack<IEnumerator<CoroutineState>>();
        }

        public void Add(IEnumerator<CoroutineState> routine) {
            routines.Add(routine);
        }

        public void Update() {

            foreach (var routine in routines) {
                var state = routine.Current;
                switch (state) {
                case CoroutineState.FINISHED:
                    toRemove.Push(routine);
                    break;
                case CoroutineState.RUNNING:
                    _ = routine.MoveNext();
                    break;
                }
            }

            while (toRemove.Count > 0) {
                _ = routines.Remove(toRemove.Pop());
            }
        }
    }

    internal enum CoroutineState {
        RUNNING,
        FINISHED,
    }

}