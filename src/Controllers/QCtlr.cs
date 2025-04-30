using System;
using System.Collections.Generic;
using UnityEngine;
using Log = KSPBuildTools.Log;

namespace MusicSwitcher.Controllers {
    /// <summary>
    /// A Controller for a simple music queue.
    /// </summary>
    internal class QCtlr : IController
    {
        private AudioSource src;
        private AudioSourceWrangler srcWrangler;
        private readonly Queue<AudioClip> clipQ;
        private bool active = true;

        public QCtlr()
        {
            
            src.volume = Statics.globalSettings.volumeMaster;
            clipQ = new Queue<AudioClip>();
        }

        public void Initialize(AudioSourceWrangler w, ConfigNode node) {
            this.srcWrangler = w;
            src = srcWrangler.Get();
            
            var cfg = ConfigNode.CreateObjectFromConfig<Config.AudioList>(node);
            foreach (var track in cfg.Load()) {
                Add(track);
            }
        }

        public void Update()
        {
            if (active && IsReadyForNextTrack())
            {
                PlayNextTrack();
            }
        }

        private bool IsReadyForNextTrack()
        {
            return !src.isPlaying && clipQ.Count != 0;
        }

        /// <summary>
        /// Watch out, does NO nullchecks
        /// </summary>
        private void PlayNextTrack()
        {
            var next = clipQ.Dequeue();
            src.clip = next;
            src.Play();
        }

        public void Add(AudioClip t)
        {
            if (active)
            {
                clipQ.Enqueue(t);
            }
        }

        public void Close()
        {
            src.Stop();
            clipQ.Clear();
            active = false;
            srcWrangler.Release(src);
        }

        public void Pause()
        {
            src.Pause();
        }

        public void UnPause()
        {
            src.UnPause();
        }

        /// <param name="vol">should be on [0,1]</param>
        public void SetVolume(float vol)
        {
            src.volume = vol * Statics.globalSettings.volumeMaster;
        }
    }
}