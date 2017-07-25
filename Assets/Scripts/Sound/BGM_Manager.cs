using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;

namespace Softdrink{



	// Singleton manager that handles BGM stuff
	// Capable of fading in/out, looping, etc.
	// Can reference a playlist of BGM Source files

	// PERSISTS BETWEEN SCENES
	//[RequireComponent(typeof(AudioSource))]
	public class BGM_Manager : MonoBehaviour {

		// Singleton Instance
		public static BGM_Manager Instance;


		[HeaderAttribute("Playback Status")]

		[ReadOnlyAttribute]
		[TooltipAttribute("READONLY: The currently playing string name of the active BGM Source.")]
		public string playingName = "";

		[ReadOnlyAttribute]
		[TooltipAttribute("READONLY: The currently playing index in the Sources list.")]
		public int playingIndex = 0;

		[ReadOnlyAttribute]
		[TooltipAttribute("READONLY: Is there any BGM playing?")]
		public bool isPlaying = false;


		[HeaderAttribute("Crossfade Status")]

		[ReadOnlyAttribute]
		[TooltipAttribute("READONLY: The name of the BGM Source being faded OUT.")]
		public string fadingName = "";

		[ReadOnlyAttribute]
		[TooltipAttribute("READONLY: The index of the BGM Source being faded OUT.")]
		public int fadingIndex = 0;

		[ReadOnlyAttribute]
		[TooltipAttribute("READONLY: Is a crossfade in progress?")]
		public bool isFading = false;


		[HeaderAttribute("Sources")]

		[TooltipAttribute("A list of all BGM Sources that this BGM Manager is currently referencing.")]
		public List<BGMSource> sources = new List<BGMSource>();


		[HeaderAttribute("Behvaior Options")]

		[TooltipAttribute("Enable or Disable Crossfades.")]
		public bool enableCrossfades = true;

		[SerializeField]
		[TooltipAttribute("The Crossfade Settings preset to use during Crossfades.")]
		private CrossfadeSettings crossfadeSettings;

		[TooltipAttribute("The Audio Mixer Group to be used for the AudioSources used to play BGM.")]
		public AudioMixerGroup audioGroup = null;

		[TooltipAttribute("Should the BGM begin playing on Awake?")]
		public bool playOnAwake = true;

		[TooltipAttribute("The List index of the BGM to begin playing On Awake. \nNOTE: This should usually be zero!")]
		public int playOnAwakeIndex = 0;


		
		private AudioSource _src = null;	// Reference to the currently active AudioSource
		private AudioSource _xsrc = null;	// Reference to currently inactive AudioSource

		private bool firstActive = true;

		private AudioSource _src0 = null;
		private AudioSource _src1 = null;

		// Timer values
		private float fadeStartTime = -5.0f;
		private float fadeEndTime = -5.0f;

		// INIT -------------------------------------------------------------------------------------------------------

		void Awake(){
			// If the Instance doesn't already exist
			if(Instance == null){
				// If the instance doesn't already exist, set it to this
				Instance = this;
			}else if(Instance != this){
				// If an instance already exists that isn't this, destroy this instance and log what happened
				Destroy(gameObject);
				Debug.LogError("ERROR! The BGM_Manager encountered another instance of BGM_Manager; it destroyed itself rather than overwrite the existing instance.", this);
			}
			DontDestroyOnLoad(transform.gameObject);

			// Set up the basic properties of the Player_Manager			
			Init();

			if(playOnAwake) Play(playOnAwakeIndex);
		}

		void Init(){
			//_src = gameObject.GetComponent<AudioSource>() as AudioSource;
			//if(_src == null) Debug.LogError("ERROR: The BGM_Manager could not associate a reference to the attached AudioSource component!", this);
			_src0 = gameObject.AddComponent<AudioSource>() as AudioSource;
			SetupAudioSource(_src0);

			if(enableCrossfades){
				_src1 = gameObject.AddComponent<AudioSource>() as AudioSource;
				SetupAudioSource(_src1);
			}

			firstActive = true;
			_src = _src0;
			if(enableCrossfades){
				_xsrc = _src1;
				_xsrc.volume = 0.0f;
			}
		}

		void SetupAudioSource(AudioSource target){
			target.loop = false;
			target.playOnAwake = false;
			target.spatialBlend = 0.0f;
			target.outputAudioMixerGroup = audioGroup;
		}

		// UPDATE ---------------------------------------------------------------------------------------------------
		
		void Update(){
			// << SUPER DEBUG >>
			if(Input.GetKeyDown(KeyCode.Space)){
				
				if(enableCrossfades){
					FadeToNext();
				}else{
					PlayNext();
				}
			}

			CheckExecuteFade();
		}


		#if UNITY_EDITOR
		void OnValidate(){
			for(int i = 0; i < sources.Count; i++){
				sources[i].Validate();
			}
		}
		#endif

		// FADE FUNCTIONS -------------------------------------------------------------------------------------------

		void FadeToNext(){
			firstActive = !firstActive;

			fadingName = playingName;
			fadingIndex = playingIndex;

			playingIndex++;
			if(playingIndex >= sources.Count) playingIndex = 0;

			if(firstActive){
				_src = _src0;
				_xsrc = _src1;
			}else{
				_src = _src1;
				_xsrc = _src0;
			}

			

			_src.clip = sources[playingIndex].source;
			playingName = sources[playingIndex].name;

			if(!_src.isPlaying || !crossfadeSettings.continuePlayInBG) _src.Play();

			fadeStartTime = Time.unscaledTime;
			fadeEndTime = Time.unscaledTime + crossfadeSettings.crossfadeDuration;
		}

		void CheckExecuteFade(){
			if(!enableCrossfades) return;

			if(Time.unscaledTime >= fadeStartTime && Time.unscaledTime <= fadeEndTime){
				if(Time.unscaledTime < fadeEndTime){
					FadeRoutine();
				}
				isFading = true;
			}else if(Time.unscaledTime >= fadeEndTime && fadeEndTime > 0f){
				if(!crossfadeSettings.continuePlayInBG) _xsrc.Stop();
				_xsrc.volume = 0.0f;

				fadeStartTime = -5.0f;
				fadeEndTime = -5.0f;

				isFading = false;
			}
		}

		float t = 0f;
		void FadeRoutine(){
			t = (Time.unscaledTime - fadeStartTime)/(crossfadeSettings.crossfadeDuration);

			_src.volume = crossfadeSettings.gain2.Evaluate(t);
			_xsrc.volume = crossfadeSettings.gain1.Evaluate(t);

			if(t <= 0f){
				_src.volume = 0f;
				_xsrc.volume = 1f;
			}
			if(t >= 1.0f){
				_src.volume = 1f;
				_xsrc.volume = 0f;
			}
		}

		// PLAY / PAUSE --------------------------------------------------------------------------------------------------

		void Play(int index){
			if(sources.Count < index + 1) return;
			if(sources[index] == null) return;

			playingName = sources[index].name;
			playingIndex = index;

			_src.clip = sources[index].source;
			if(!_src.isPlaying) _src.Play();
			else _src.UnPause();
			if(enableCrossfades) _xsrc.Stop();

			isPlaying = true;
		}

		void PlayNext(){
			playingIndex++;
			if(playingIndex >= sources.Count) playingIndex = 0;
			Play(playingIndex);
		}

		void Pause(){
			_src.Pause();
			if(isFading) _xsrc.Pause();
		}



	}
}