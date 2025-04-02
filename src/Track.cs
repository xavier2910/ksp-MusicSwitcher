using System;
using System.Collections.Generic;
using UnityEngine;

namespace MusicSwitcher
{
    [Serializable]
    public class Track
    {
        public bool loop;
        public AudioClip clip;

        public Track(AudioClip clip, bool loop)
        {
            this.clip = clip;
            this.loop = loop;
        }
    }
}