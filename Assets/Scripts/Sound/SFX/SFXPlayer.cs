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

		// Play the current selected SFX
		public void Play(){
			_src.Play();
		}

		// Play the current selected SFX as a One Shot
		public void PlayOneShot(){
			_src.PlayOneShot(SFX.sources[0], SFX.settings.volume);
		}

		// Stop playing, if not in One Shot mode
		public void Stop(){

		}

		// Immediately stop playing SFX, including One Shots
		public void HardStop(){

		}
	}
}