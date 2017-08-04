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

		[ReadOnlyAttribute]
		[Range(0f,1f)]
		[TooltipAttribute("READONLY: The current playback progress of the active BGM Source.")]
		public float playbackProgress = 0.0f;


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

		[ReadOnlyAttribute]
		[Range(0f,1f)]
		[TooltipAttribute("READONLY: The current playback progress of the faded BGM Source.")]
		public float fadedPlaybackProgress = 0.0f;


		[HeaderAttribute("Sources")]

		[TooltipAttribute("A list of all BGM Sources that this BGM Manager is currently referencing.")]
		public List<BGMSource> sources = new List<BGMSource>();


		[HeaderAttribute("Behvaior Options")]

		[TooltipAttribute("Exit the loop at the end of this iteration, if applicable.")]
		public bool exitLoop = false;

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

		[SerializeField]
		private BGMSource _srcTrack = null;	// Reference to currently active BGMSource
		[SerializeField]
		private BGMSource _xsrcTrack = null;

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
			DontDestroyOnLoad(transform.root);

			// Set up the basic properties of the Player_Manager			
			Init();

			// if(playOnAwake) Play(playOnAwakeIndex);
			// if(playOnAwake) Play("BGM_Arcade");
			if(playOnAwake) Play("BGM_Arabesque", "BGM_Arcade");
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

			CheckLooping();
			CheckExecuteFade();
			UpdatePlayheads();

		}


		#if UNITY_EDITOR
		void OnValidate(){
			for(int i = 0; i < sources.Count; i++){
				sources[i].Validate();
			}
		}
		#endif

		void UpdatePlayheads(){
			playbackProgress = _src.time /_src.clip.length;
			if(enableCrossfades){
				if(_xsrc.isPlaying)
					fadedPlaybackProgress = _xsrc.time / _xsrc.clip.length;
				else fadedPlaybackProgress = 0f;
			}
		}

		// LOOPING --------------------------------------------------------------------------------------------------

		void CheckLooping(){
			CheckLoop(_src, _srcTrack);
			if(enableCrossfades){
				CheckLoop(_xsrc, _xsrcTrack);
			}
		}

		void CheckLoop(AudioSource target, BGMSource track){
			if(target == null || track == null) return;
			if(exitLoop){
					if(target.time >= track.source.length){
						exitLoop = false;
						target.Stop();
					}
					if(target.loop) target.loop = false;
					return;
				}
			switch(track.loopMode){
				case BGMLoopMode.None:
					if(target.loop) target.loop = false;
					break;
				case BGMLoopMode.LoopEntire:
					if(!target.loop) target.loop = true;
					break;
				case BGMLoopMode.LoopFromStart:
					if(target.loop) target.loop = false;
					if(target.time >= track.source.length){
						target.time = track.loopStartTime;
						//target.SetScheduledEndTime(AudioSettings.dspTime + (track.source.length - track.loopStartTime));
					}
					break;
				case BGMLoopMode.LoopToEnd:
					if(target.loop) target.loop = false;
					if(target.time >= track.loopEndTime){
						target.time = 0.0f;
						//target.SetScheduledEndTime(AudioSettings.dspTime + (track.loopEndTime));
					}
					break;
				case BGMLoopMode.LoopFromStartToEnd:
					if(target.loop) target.loop = false;
					if(target.time >= track.loopEndTime){
						target.time = track.loopStartTime;
					}
					break;
			}
		}

		// FADE FUNCTIONS -------------------------------------------------------------------------------------------

		// FADE TO NEXT IN LIST ---------------------------------------
		void FadeToNext(){
			firstActive = !firstActive;

			fadingName = playingName;
			fadingIndex = playingIndex;

			playingIndex++;
			if(playingIndex >= sources.Count) playingIndex = 0;

			SwitchSources();

			_xsrcTrack = _srcTrack;
			_srcTrack = sources[playingIndex];

			_src.clip = sources[playingIndex].source;
			playingName = sources[playingIndex].name;

			if(!_src.isPlaying || !crossfadeSettings.continuePlayInBG) _src.Play();

			fadeStartTime = Time.unscaledTime;
			fadeEndTime = Time.unscaledTime + crossfadeSettings.crossfadeDuration;
		}

		// FADE TO A SPECIFIED TRACK
		void FadeToLocal(int index){

		}

		void FadeToLocal(string name){

		}

		public static void FadeTo(int index){
			Instance.FadeToLocal(index);
		}


		// CROSSFADE BETWEEN ACTIVE AND CUED TRACKS
		void CrossfadeLocal(){

		}

		public static void Crossfade(){
			Instance.CrossfadeLocal();
		}

		// EXECUTE FADES
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
				_src.volume = 1.0f;

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

		// PLAY --------------------------------------------------------------------------------------------------

		void Play(int index){
			if(sources.Count < index + 1) return;
			if(sources[index] == null) return;

			if(enableCrossfades){
				

				fadingName = playingName;
				fadingIndex = playingIndex;

				SwitchSources();
				MuteInactive();

				_xsrcTrack = _srcTrack;

				firstActive = !firstActive;
			}

			playingName = sources[index].name;
			playingIndex = index;
			_srcTrack = sources[index];

			_src.clip = sources[index].source;
			if(!_src.isPlaying) _src.Play();
			else _src.UnPause();
			if(enableCrossfades) _xsrc.Stop();

			isPlaying = true;
		}

		// Cues index1 and begins playing index0
		void Play(int index0, int index1){
			Play(index1);
			Play(index0);
		}

		void PlayNext(){
			playingIndex++;
			if(playingIndex >= sources.Count) playingIndex = 0;
			Play(playingIndex);
		}

		public static void PlayByIndex(int index){
			Instance.Play(index);
		}

		public static void PlayByIndex(int index0, int index1){
			Instance.Play(index0, index1);
		}

		void Play(string name){
			int t = FindSourceByName(name);
			if(t != -1){
				playingIndex = t;
				playingName = name;
				Play(playingIndex);
			}
		}

		public static void PlayByName(string name){
			Instance.Play(name);
		}

		void Play(string name0, string name1){
			Play(name1);
			Play(name0);
		}

		public static void PlayByName(string name0, string name1){
			Instance.Play(name0, name1);
		}

		// CUE, PAUSE, STOP, EXIT LOOP ------------------------------------------------------------------------------------

		void Cue(int index){
			if(!enableCrossfades) return;
			if(sources.Count < index + 1) return;
			if(sources[index] == null) return;

			_xsrcTrack = sources[index];
			fadingIndex = index;
			fadingName = _xsrcTrack.Name;

			_xsrc.Stop();
			_xsrc.clip = _xsrcTrack.source;
		}
		

		void PauseLocal(){
			_src.Pause();
			if(isFading) _xsrc.Pause();
		}

		public static void Pause(){
			Instance.PauseLocal();
		}

		void StopLocal(){
			_src.Stop();
			if(isFading) _xsrc.Stop();
		}

		public static void Stop(){
			Instance.StopLocal();
		}

		public static void ExitLoop(){
			Instance.exitLoop = true;
		}


		// UTILITY / HELPER METHODS -------------------------------------------------------------------

		int FindSourceByName(string name){
			for(int i = 0; i < sources.Count; i++){
				if(sources[i].Name.Equals(name)){
					return i;
				}
			}

			return -1;
		}

		void SwitchSources(){
			if(firstActive){
				_src = _src0;
				_xsrc = _src1;
			}else{
				_src = _src1;
				_xsrc = _src0;
			}
		}

		void MuteInactive(){
			_src.volume = 1.0f;
			_xsrc.volume = 0.0f;
		}

	}
}