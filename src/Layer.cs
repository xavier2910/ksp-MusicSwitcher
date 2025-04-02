using System;
using System.Collections.Generic;
using UnityEngine;

namespace MusicSwitcher
{
    /// <summary>
    /// A wrapper around an AudioSource and a TQ.
    /// </summary>
    internal class Layer
    {
        private AudioSource src;
        private TrackQueue clipQ;

        public Layer(AudioSource src)
        {
            this.src = src;
            clipQ = new TrackQueue();
        }

        public bool IsReadyForNextTrack()
        {
            return !src.isPlaying && !clipQ.IsEmpty();
        }

        /// <summary>
        /// Watch out, does NO nullchecks
        /// </summary>
        public void PlayNextTrack() {
            var next = clipQ.Dequeue();
            src.loop = next.loop;
            src.clip = next.clip;
            src.Play();
        }

        public void Enqueue(Track t) {
            clipQ.Enqueue(t);
        }

        public void Reset() {
            Stop();
            clipQ.Clear();
        }

        public void Stop()
        {
            src.Stop();
        }

        public void Pause()
        {
            src.Pause();
        }

        public void UnPause()
        {
            src.UnPause();
        }
    }
}