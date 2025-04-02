using System;
using System.Collections.Generic;
using UnityEngine;

namespace MusicSwitcher
{
    /// <summary>
    /// The idea here is that other, scene-dependent components will use this
    /// to queue up music tracks. It can be accessed via the Statics.GetSwitcher()
    /// method.
    /// </summary>
    public class MusicSwitcher : MonoBehaviour {

        private Dictionary<int, Layer> layers;
        // we hang on to this because it has all the default clips loaded into it
        // and so we can treat it as our little clip-hoard.
        private MusicLogic tameMusicLogic;

        private void Awake() 
        {
            HijackMusicLogic();
            ExtractStaticClips();
            SetGlobalInstance();
        }

        private void HijackMusicLogic()
        {
            tameMusicLogic = GetComponent<MusicLogic>();
            tameMusicLogic.enabled = false;

            var asrcs = GetComponents<AudioSource>();
            foreach (var asrc in asrcs)
            {
                asrc.enabled = false;
            }
            GetComponent<DetonatorDriver>().enabled = false;
            var phs = GetComponents<PauseAudioFadeHandler>();
            foreach (var ph in phs)
            {
                ph.enabled = false;
            }
        }

        private void ExtractStaticClips()
        {
            Statics.Clips.mainMenuAmbience = tameMusicLogic.menuAmbience;
            Statics.Clips.mainMenuTheme = tameMusicLogic.menuTheme;
        }

        private void SetGlobalInstance()
        {
            Statics.switcherInstance = this;
        }

        private void Start() {
            gameObject.tag = Statics.kMusicSwitcherTag;
            layers = new Dictionary<int, Layer>(Statics.kNLayersDefault);
        }

        private void Update() {
            foreach (var layer in layers.Values)
            {
                if (layer.IsReadyForNextTrack())
                {
                    layer.PlayNextTrack();
                }
            }
        }

        /// <summary>
        /// This is how you tell the thing to play music, by adding the given track
        /// to the queue on layer `layer`. To skip to a particular track, call
        /// `Clear()` and then this.
        /// </summary>
        public void Enqueue(Track t, int layer = 0)
        {
            GetLayer(layer).Enqueue(t);
        }

        /// <summary>
        /// Clears the given layer's queue and stops all music on that layer.
        /// </summary>
        public void Clear(int layer = 0)
        {
            GetLayer(layer).Reset();
        }

        /// <summary>
        /// Pretty self-explanitory I think.
        /// </summary>
        public void ClearAll()
        {
            foreach (var layer in layers.Values)
            {
                layer.Reset();
            }
        }

        private Layer GetLayer(int layer)
        {
            try
            {
                _ = layers[layer];
            }
            catch (KeyNotFoundException)
            {
                NewLayer(layer);
            }

            return layers[layer];
        }

        /// <summary>As it stands, it does NOT check if the layer already exists</summary>
        private void NewLayer(int layer)
        {
            var newSrc = gameObject.AddComponent<AudioSource>();
            newSrc.spatialBlend = 0.0F;
            layers[layer] = new Layer(newSrc);
        }
    }
}