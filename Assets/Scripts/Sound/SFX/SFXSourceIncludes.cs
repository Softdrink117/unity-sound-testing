﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Softdrink{
	// Struct containing playback data for an SFXSource (or other randomizable audio)
	[System.Serializable]
	public struct SFXPlaybackSettings{

		[Range(0f,1f)]
		[TooltipAttribute("The playback volume for this sound effect.")]
		public float volume;

		[TooltipAttribute("How much volume randomization to apply when playing back.")]
		public float volumeRandom;

		[TooltipAttribute("How much pitch randomization to apply when playing back.")]
		public float pitchRandomization;

		[TooltipAttribute("Should the SFX loop on play?")]
		public bool loop;

		[Range(0f,1f)]
		[TooltipAttribute("Spatial blend setting. \n0 = 2D, 1 = 3D.")]
		public float spatial;

		public SFXPlaybackSettings(float volIn, float volRandIn, float pitchRandIn, bool lp, float space){
			volume = volIn;
			volumeRandom = volRandIn;
			pitchRandomization = pitchRandIn;
			loop = lp;
			spatial = space;
		}

		public void Validate(){
			if(volume < 0f) volume = 0f;
			if(volume > 1f) volume = 1f;
			if(spatial < 0f) spatial = 0f;
			if(spatial > 1f) spatial = 1f;
		}

		public void SetDefault(){
			volume = 1.0f;
			volumeRandom = 0.0f;
			pitchRandomization = 0.0f;
			loop = false;
			spatial = 0.0f;
		}
	};
}