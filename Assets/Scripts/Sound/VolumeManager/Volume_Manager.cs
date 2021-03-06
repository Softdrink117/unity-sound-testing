﻿using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

namespace Softdrink{

	// Singleton manager that interfaces with the
	// AudioMixer to control volume globally through
	// the whole project. Easily accessible from UI, etc.

	// PERSISTS BETWEEN SCENES

	[AddComponentMenu("Scripts/Sound/Volume Manager")]
	public class Volume_Manager : MonoBehaviour {

		// Singleton Instance
		public static Volume_Manager Instance;

		
		[SerializeField]
		[TooltipAttribute("The parameter reference settings for the Audio Mixer.")]
		private VolumeManagerSettings settings;
		

		[HeaderAttribute("Volume Controls")]

		[RangeAttribute(-80f, 0f)]
		[TooltipAttribute("Master volume (dB).")]
		public float masterVolume = 0f;

		[TooltipAttribute("Master muted?")]
		public bool masterMute = false;

		[RangeAttribute(-80f, 0f)]
		[TooltipAttribute("Music volume (dB).")]
		public float musicVolume = 0f;

		[TooltipAttribute("Music muted?")]
		public bool musicMute = false;

		[RangeAttribute(-80f, 0f)]
		[TooltipAttribute("SFX volume (dB).")]
		public float sfxVolume = 0f;

		[TooltipAttribute("SFX muted?")]
		public bool sfxMute = false;

		[RangeAttribute(-80f, 0f)]
		[TooltipAttribute("Voice volume (dB).")]
		public float voiceVolume = 0f;

		[TooltipAttribute("Voice muted?")]
		public bool voiceMute = false;

		private bool initialized = false;

		void Awake () {
			// If the Instance doesn't already exist
			if(Instance == null){
				// If the instance doesn't already exist, set it to this
				Instance = this;
			}else if(Instance != this){
				// If an instance already exists that isn't this, destroy this instance and log what happened
				Destroy(gameObject);
				Debug.LogError("ERROR! The Volume_Manager encountered another instance of Volume_Manager; it destroyed itself rather than overwrite the existing instance.", this);
			}
			DontDestroyOnLoad(transform.root);

			CheckInit();
		}

		void CheckInit(){
			if(settings.mixer != null && !settings.masterParameter.Equals("") && !settings.musicParameter.Equals("") && !settings.sfxParameter.Equals("") && !settings.voiceParameter.Equals("")){
				initialized = true;
			}else{
				#if UNITY_EDITOR
					Debug.LogError("ERROR! Volume_Manager was not properly initialized!");
				#endif
			}

			if(initialized) GetInitialValues();
		}

		// Load the values from somewhere. Current behavior loads from the Mixer,
		// but in a proper game, would probably read from a .cfg file containing
		// volume settings as defined by user.
		void GetInitialValues(){
			// LOAD FROM CFG FILE or PLAYERPREFS, etc.
			//LoadFromFileTesterHurDurr();
			GetFromMixer();
		}

		void GetFromMixer(){
			settings.mixer.GetFloat(settings.masterParameter, out masterVolume);
			settings.mixer.GetFloat(settings.musicParameter, out musicVolume);
			settings.mixer.GetFloat(settings.sfxParameter, out sfxVolume);
			settings.mixer.GetFloat(settings.voiceParameter, out voiceVolume);
		}
		
		void Update () {
		
		}

		#if UNITY_EDITOR
			void OnValidate(){
				if(!masterMute) SetMaster(masterVolume);
				else SetMaster(-80);
				if(!musicMute) SetMusic(musicVolume);
				else SetMusic(-80);
				if(!sfxMute) SetSFX(sfxVolume);
				else SetSFX(-80);
				if(!voiceMute) SetVoice(voiceVolume);
				else SetVoice(-80);
			}
		#endif

		// MIXER IO METHODS ======================================

		// Master ------------------------------

		public void SetMaster(float volume){
			if(!initialized) return;
			masterVolume = volume;
			settings.mixer.SetFloat(settings.masterParameter, volume);
		}

		public static void SetMasterVol(float volume){
			Instance.SetMaster(volume);
		}

		public void MasterMute(){
			masterMute = !masterMute;
			if(masterMute) settings.mixer.SetFloat(settings.masterParameter, -80f);
			else SetMaster(masterVolume);
		}

		public static void ToggleMasterMute(){
			Instance.MasterMute();
		}

		// Music -----------------------------------

		public void SetMusic(float volume){
			if(!initialized) return;
			musicVolume = volume;
			settings.mixer.SetFloat(settings.musicParameter, volume);
		}

		public static void SetMusicVol(float volume){
			Instance.SetMusic(volume);
		}

		public void MusicMute(){
			musicMute = !musicMute;
			if(musicMute) settings.mixer.SetFloat(settings.musicParameter, -80f);
			else SetMusic(musicVolume);
		}

		public static void ToggleMusicMute(){
			Instance.MusicMute();
		}

		// SFX --------------------------------------

		public void SetSFX(float volume){
			if(!initialized) return;
			sfxVolume = volume;
			settings.mixer.SetFloat(settings.sfxParameter, volume);
		}

		public static void SetSFXVol(float volume){
			Instance.SetSFX(volume);
		}

		public void SFXMute(){
			sfxMute = !sfxMute;
			if(sfxMute) settings.mixer.SetFloat(settings.sfxParameter, -80f);
			else SetSFX(sfxVolume);
		}

		public static void ToggleSFXMute(){
			Instance.SFXMute();
		}

		// Voice ---------------------------------------

		public void SetVoice(float volume){
			if(!initialized) return;
			voiceVolume = volume;
			settings.mixer.SetFloat(settings.voiceParameter, volume);
		}

		public static void SetVoiceVol(float volume){
			Instance.SetVoice(volume);
		}

		public void VoiceMute(){
			voiceMute = !voiceMute;
			if(voiceMute) settings.mixer.SetFloat(settings.voiceParameter, -80f);
			else SetVoice(voiceVolume);
		}

		public static void ToggleVoiceMute(){
			Instance.VoiceMute();
		}
	}
}