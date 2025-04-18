using System;
using System.Collections.Generic;
using UnityEngine;
using Log = KSPBuildTools.Log;

namespace MusicSwitcher {
    /// <summary>
    /// The idea here is that other, scene-dependent components will use this
    /// to queue up music tracks. It can be accessed via the Statics.switcherInstance
    /// property.
    /// </summary>
    public class MusicSwitcher : MonoBehaviour {

        private Dictionary<int, Layer> layers;
        private StockMusicExtractor extractor;
        private bool usingStockMusic = false;

        #region UnityMessages

        private void Awake() 
        {
            extractor = gameObject.AddComponent<StockMusicExtractor>();
            TakeOver();
            SetGlobalInstance();
        }

        private void SetGlobalInstance()
        {
            Statics.switcherInstance = this;
        }

        private void Start() {
            layers = new Dictionary<int, Layer>();
        }

        private void Update() {
            if (!usingStockMusic) {
                foreach (var layer in layers.Values)
                {
                    if (layer.IsReadyForNextTrack())
                    {
                        layer.PlayNextTrack();
                    }
                }
            }
        }

        private void OnDestroy() {
            Log.Debug("being destroyed -- bye!");
        }

        #endregion
        #region PublicMethods

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

        public void SetVolume(float vol, int layer = 0) {
            GetLayer(layer).SetVolume(vol);
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

        public void StopAll()
        {
            foreach (var layer in layers.Values)
            {
                layer.Stop();
            }
        }

        public void PauseAll() {
            foreach (var layer in layers.Values)
            {
                layer.Pause();
            }
        }

        public void UnPauseAll() {
            foreach (var layer in layers.Values)
            {
                layer.UnPause();
            }
        }

        /// <summary>
        /// Reverts music to the control of the stock MusicLogic for
        /// those who so desire
        /// </summary>
        /// <param name="enableCrickets">
        /// So funny story, in some scenes the audio2 audio source doesn't get
        /// properly dealt with by the stock MusicLogic, and so you end up with
        /// a cricket infestation in the Tracking Station. The default is false; you
        /// need only bother with this if there are missing crickets.
        /// </param>
        public void RevertToStock(bool enableCrickets = false)
        {
            Log.Message("Reverting to stock MusicLogic");
            usingStockMusic = true;
            extractor.tameMusicLogic.audio1.enabled = true;
            extractor.tameMusicLogic.audio2.enabled = enableCrickets;
            extractor.tameMusicLogic.enabled = true;
            var phs = GetComponents<PauseAudioFadeHandler>();
            foreach (var ph in phs)
            {
                ph.enabled = true;
            }
        }

        public void TakeOver()
        {
            Log.Message("Taking command of the music!");
            usingStockMusic = false;
            extractor.tameMusicLogic.enabled = false;
            extractor.tameMusicLogic.audio1.enabled = false;
            extractor.tameMusicLogic.audio2.enabled = false;
            var phs = GetComponents<PauseAudioFadeHandler>();
            foreach (var ph in phs)
            {
                ph.enabled = false;
            }
        }

        #endregion
        #region PrivateHelpers

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

        #endregion
    }
}