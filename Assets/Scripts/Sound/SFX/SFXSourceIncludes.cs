using System.Collections;
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

		public SFXPlaybackSettings(float volIn, float volRandIn, float pitchRandIn){
			volume = volIn;
			volumeRandom = volRandIn;
			pitchRandomization = pitchRandIn;
		}

		public void Validate(){
			if(volume < 0f) volume = 0f;
		}
	};
}