using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Softdrink{
	// System to play Sound Effects
	[AddComponentMenu("Scripts/Sound/SFXPlayer")]
	public class SFXPlayer : MonoBehaviour {

		[SerializeField]
		[TooltipAttribute("The currently selected SFXSource to be played.")]
		private SFXSource SFX = null;

		private AudioSource _src = null;

		///// INIT

		void Awake(){
			GetReferences();
		}

		void GetReferences(){
			if(_src == null)
				_src = gameObject.GetComponent<AudioSource>() as AudioSource;
			if(_src == null)
				_src = gameObject.AddComponent<AudioSource>() as AudioSource;
			if(_src == null)
				Debug.LogError("ERROR! SFXPlayer could not associate a reference to an AudioSource.", this);
			
		}

		///// MAIN METHODS

		// Apply properties to the AudioSource
		void ApplyProperties(){
			_src.pitch = SFX.settings.GetPitch();
			_src.loop = SFX.settings.loop;
			_src.spatialBlend = SFX.settings.spatial;
		}

		// Play the current selected SFX
		public void Play(){
			ApplyProperties();
			_src.clip = SFX.GetClip();
			_src.Play();
		}

		// Play the current selected SFX as a One Shot
		public void PlayOneShot(){
			ApplyProperties();
			_src.PlayOneShot(SFX.GetClip(), SFX.settings.GetVolume());
		}

		// Stop playing, if not in One Shot mode
		public void Stop(){
			_src.Stop();
		}

		// Immediately stop playing SFX, including One Shots
		public void HardStop(){
			_src.enabled = false;
			_src.enabled = true;
		}
	}
}