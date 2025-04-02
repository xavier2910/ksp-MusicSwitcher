using System;
using System.Collections.Generic;
using UnityEngine;

namespace MusicSwitcher
{
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
            src.Stop();
            clipQ.Clear();
        }
    }
}