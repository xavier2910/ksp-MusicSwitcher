using System;
using System.Collections.Generic;
using UnityEngine;

namespace MusicSwitcher
{
    /// <summary>
    /// This MB's sole purpose in life is to get the stock music clips out of the
    /// disabled Squad MusicLogic component.
    /// </summary>
    public class StockMusicExtractor : MonoBehaviour {
        
        // our dear little pet MusicLogic. We're going to dissect it under
        // anaesthesia so we don't lose the stock music altogether.
        private MusicLogic tameMusicLogic;

        private void Awake() {
            TameMusicLogic();
            ExtractMainMenuClips();
        }

        private void TameMusicLogic() {
            tameMusicLogic = GetComponent<MusicLogic>();
            tameMusicLogic.enabled = false;
            // don't worry, you won't feel a thing...
        }

        private void ExtractMainMenuClips()
        {
            Statics.Clips.mainMenuAmbience = tameMusicLogic.menuAmbience;
            Statics.Clips.mainMenuTheme = tameMusicLogic.menuTheme;
        }

        // also includes tracking station and other buildings
        // nb does not include editor music
        private void ExtractKSCAmbience()
        {
            Statics.Clips.kscDay = tameMusicLogic.spaceCenterAmbienceDay;
            Statics.Clips.kscNight = tameMusicLogic.spaceCenterAmbienceNight;
            Statics.Clips.trackingStation = tameMusicLogic.trackingAmbience;
            Statics.Clips.rnd = tameMusicLogic.researchComplexAmbience;
            Statics.Clips.missionctl = tameMusicLogic.missionControlAmbience;
            Statics.Clips.admin = tameMusicLogic.adminFacilityAmbience;
            Statics.Clips.astroComplex = tameMusicLogic.astroComplexAmbience;
            Statics.Clips.sph = tameMusicLogic.SPHAmbience;
            Statics.Clips.vab = tameMusicLogic.VABAmbience;
        }
    }
}