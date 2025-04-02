using System;
using System.Collections.Generic;
using UnityEngine;

namespace MusicSwitcher
{    
    public class MusicSwitcher : MonoBehaviour {

        private AudioSource layer1;
        private AudioSource layer2;
        private AudioSource layer3;

        private MusicLogic tameMusicLogic;

        private void Awake() {
            tameMusicLogic = GetComponent<MusicLogic>();
            tameMusicLogic.enabled = false;

            var asrcs = GetComponents<AudioSource>();
            foreach (var asrc in asrcs)
            {
                asrc.enabled = false;
            }
        }

        private void Start() {
            layer1 = gameObject.AddComponent<AudioSource>();
            layer2 = gameObject.AddComponent<AudioSource>();
            layer3 = gameObject.AddComponent<AudioSource>();

            tag = Statics.kMusicSwitcherTag;
        }
    }
}