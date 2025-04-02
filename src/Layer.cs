using System;
using System.Collections.Generic;
using UnityEngine;

namespace MusicSwitcher
{
    internal class Layer
    {
        private AudioSource src;
        private Queue<Track> clipQ;

        public Layer(AudioSource src)
        {
            this.src = src;
            clipQ = new Queue<Track>();
        }

        public bool IsReadyForNextTrack()
        {
            bool clipAvailable = false;
            try
            {
                clipQ.Peek();
                clipAvailable = true;
            }
            catch (InvalidOperationException)
            {
                // nothing need happen -- we're just making a bool
            }
            return !src.isPlaying && clipAvailable;
        }

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