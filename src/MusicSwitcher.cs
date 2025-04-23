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

        private List<MusicController> activeControllers;
        private StockMusicExtractor extractor;
        private bool usingStockMusic = false;

        public AudioSourceWrangler SourceWrangler {get; private set;}

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
            activeControllers = new List<MusicController>();
            SourceWrangler = new AudioSourceWrangler(gameObject);
        }

        private void Update() {
            if (!usingStockMusic) {
                foreach (var ctlr in activeControllers)
                {
                    ctlr.Update();
                }
            }
        }

        private void OnDestroy() {
            Log.Debug("being destroyed -- bye!");
        }

        #endregion
        #region PublicMethods

        public void Register(MusicController ctlr) {
            activeControllers.Add(ctlr);
        }

        /// <summary>
        /// Pretty self-explanitory I think.
        /// </summary>
        public void ClearAll()
        {
            foreach (var ctlr in activeControllers)
            {
                ctlr.Close();
            }
            activeControllers.Clear();
            SourceWrangler.ReleaseAll();
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
            // I really have no idea what these do, but they probably do
            // something, else they'd not be there, right?
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
            // I really have no idea what these do, but why risk it?
            var phs = GetComponents<PauseAudioFadeHandler>();
            foreach (var ph in phs)
            {
                ph.enabled = false;
            }
        }

        #endregion
        #region PrivateHelpers


        #endregion
    }
}