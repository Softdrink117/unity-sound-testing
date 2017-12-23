using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;

namespace Softdrink{



	// Singleton manager that handles BGM stuff
	// Capable of fading in/out, looping, etc.
	// Can reference a playlist of BGM Source files

	// PERSISTS BETWEEN SCENES
	[AddComponentMenu("Scripts/Sound/BGM Manager")]
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

		//[ReadOnlyAttribute]
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

		//[ReadOnlyAttribute]
		[Range(0f,1f)]
		[TooltipAttribute("READONLY: The current playback progress of the faded BGM Source.")]
		public float fadedPlaybackProgress = 0.0f;


		[HeaderAttribute("Sources")]

		[TooltipAttribute("A list of all BGM Sources that this BGM Manager is currently referencing.")]
		public List<BGMSource> sources = new List<BGMSource>();


		[HeaderAttribute("Behvaior Options")]

		[TooltipAttribute("Exit the loop at the end of this iteration, if applicable.")]
		public bool exitLoop = false;

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

		private BGMSource _srcTrack = null;	// Reference to currently active BGMSource
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

			if(playOnAwake) Play(playOnAwakeIndex);
			// if(playOnAwake) Play("BGM_Arcade");
			// if(playOnAwake) Play("BGM_Arabesque", "BGM_Arcade");
			// if(playOnAwake) Play(1,0);
			// if(playOnAwake) FadeOutLocal(1, 8.0f);
		}

		void Init(){
			//_src = gameObject.GetComponent<AudioSource>() as AudioSource;
			//if(_src == null) Debug.LogError("ERROR: The BGM_Manager could not associate a reference to the attached AudioSource component!", this);
			_src0 = gameObject.AddComponent<AudioSource>() as AudioSource;
			SetupAudioSource(_src0);

			_src1 = gameObject.AddComponent<AudioSource>() as AudioSource;
			SetupAudioSource(_src1);
			
			firstActive = true;
			_src = _src0;
			
			_xsrc = _src1;
			_xsrc.volume = 0.0f;
			
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
				//FadeToNext();
				//PlayNext();
				CrossfadeLocal();
				//FadeToLocal(fadingIndex, 3.0f);
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
			if(_xsrc.isPlaying)
				fadedPlaybackProgress = _xsrc.time / _xsrc.clip.length;
			else fadedPlaybackProgress = 0f;
		}

		// LOOPING --------------------------------------------------------------------------------------------------

		void CheckLooping(){
			CheckLoop(_src, _srcTrack);
			CheckLoop(_xsrc, _xsrcTrack);
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

		bool isCrossfade = false;

		// EXECUTE FADES
		void CheckExecuteFade(){
			if(Time.unscaledTime >= fadeStartTime && Time.unscaledTime <= fadeEndTime){
				if(Time.unscaledTime < fadeEndTime){
					if(isCrossfade) CrossfadeRoutine();
					else FadeRoutine();
				}
				isFading = true;
			}else if(Time.unscaledTime >= fadeEndTime && fadeEndTime > 0f){
				if(isCrossfade) EndCrossfade();
				else EndFade();
			}
		}

		float t = 0f;
		bool fadingIn = true;

		void FadeRoutine(){
			t = (Time.unscaledTime - fadeStartTime)/(fadeEndTime - fadeStartTime);

			if(fadingIn) _src.volume = crossfadeSettings.fadeIn.Evaluate(t);
			else _src.volume = crossfadeSettings.fadeOut.Evaluate(t);

			if(t <= 0f){
				if(fadingIn) _src.volume = 0.0f;
				else _src.volume = 1.0f;
			}
			if(t >= 1.0f){
				if(fadingIn) _src.volume = 1.0f;
				else _src.volume = 0.0f;
			}
		}

		void EndFade(){
			if(fadingIn){
				_src.volume = 1.0f;
				_xsrc.volume = 0.0f;
			}else{
				_src.volume = 0.0f;
				_xsrc.volume = 0.0f;
			}

			if(!crossfadeSettings.continuePlayInBG) _xsrc.Stop();

			fadeStartTime = -5.0f;
			fadeEndTime = -5.0f;

			isFading = false;
			isCrossfade = false;
		}

		void CrossfadeRoutine(){
			t = (Time.unscaledTime - fadeStartTime)/(fadeEndTime - fadeStartTime);

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

		void EndCrossfade(){
			if(!crossfadeSettings.continuePlayInBG) _xsrc.Stop();
			_xsrc.volume = 0.0f;
			_src.volume = 1.0f;

			fadeStartTime = -5.0f;
			fadeEndTime = -5.0f;

			isFading = false;
			isCrossfade = false;
		}

		// FADE IN / OUT FUNCTIONS ------------------------------------

		// FADE IN ----------------------------------------------------
		void FadeInLocal(int index, float duration){
			if(!CheckIndexRange(index)) return;
			playingIndex = index;
			_srcTrack = sources[playingIndex];
			_src.clip = sources[playingIndex].source;
			playingName = sources[playingIndex].Name;

			if(!_src.isPlaying || !crossfadeSettings.continuePlayInBG) _src.Play();
			_src.volume = 0.0f;

			fadeStartTime = Time.unscaledTime;
			fadeEndTime = Time.unscaledTime + duration;

			isFading = true;
			isCrossfade = false;
			fadingIn = true;
		}

		void FadeInLocal(int index){
			FadeInLocal(index, crossfadeSettings.fadeDuration);
		}

		void FadeInLocal(string name){
			int t = FindSourceByName(name);
			if(t == -1) return;
			FadeInLocal(t, crossfadeSettings.fadeDuration);
		}

		void FadeInLocal(string name, float duration){
			int t = FindSourceByName(name);
			if(t == -1) return;
			FadeInLocal(t, duration);
		}

		public static void FadeIn(int index){
			Instance.FadeInLocal(index);
		}

		public static void FadeIn(int index, float duration){
			Instance.FadeInLocal(index, duration);
		}

		public static void FadeIn(string name){
			Instance.FadeInLocal(name);
		}

		public static void FadeIn(string name, float duration){
			Instance.FadeInLocal(name, duration);
		}

		// FADE OUT ----------------------------------------------------
		void FadeOutLocal(int index, float duration){
			if(!CheckIndexRange(index)) return;
			playingIndex = index;
			_srcTrack = sources[playingIndex];
			_src.clip = sources[playingIndex].source;
			playingName = sources[playingIndex].Name;

			if(!_src.isPlaying || !crossfadeSettings.continuePlayInBG) _src.Play();
			_src.volume = 1.0f;

			fadeStartTime = Time.unscaledTime;
			fadeEndTime = Time.unscaledTime + duration;

			isFading = true;
			isCrossfade = false;
			fadingIn = false;
		}

		void FadeOutLocal(int index){
			FadeOutLocal(index, crossfadeSettings.fadeDuration);
		}

		void FadeOutLocal(string name){
			int t = FindSourceByName(name);
			if(t == -1) return;
			FadeOutLocal(t, crossfadeSettings.fadeDuration);
		}

		void FadeOutLocal(string name, float duration){
			int t = FindSourceByName(name);
			if(t == -1) return;
			FadeOutLocal(t, duration);
		}

		public static void FadeOut(int index){
			Instance.FadeOutLocal(index);
		}

		public static void FadeOut(int index, float duration){
			Instance.FadeOutLocal(index, duration);
		}

		public static void FadeOut(string name){
			Instance.FadeOutLocal(name);
		}

		public static void FadeOut(string name, float duration){
			Instance.FadeOutLocal(name, duration);
		}

		// CROSSFADE FUNCTIONS --------------------------------------------------------------------------------------

		// FADE TO NEXT IN LIST ---------------------------------------
		void FadeToNext(){
			int t = playingIndex;
			t++;
			if(t >= sources.Count) t = 0;
			FadeToLocal(t);
		}

		// FADE TO A SPECIFIED TRACK
		void FadeToLocal(int index, float duration){
			if(!CheckIndexRange(index)) return;

			firstActive = !firstActive;

			fadingName = playingName;
			fadingIndex = playingIndex;

			playingIndex = index;

			SwitchSources();

			_xsrcTrack = _srcTrack;
			_srcTrack = sources[playingIndex];

			_src.clip = sources[playingIndex].source;
			playingName = sources[playingIndex].Name;

			if(!_src.isPlaying || !crossfadeSettings.continuePlayInBG) _src.Play();

			fadeStartTime = Time.unscaledTime;
			fadeEndTime = Time.unscaledTime + duration;
			
			isCrossfade = true;
		}

		void FadeToLocal(int index){
			FadeToLocal(index, crossfadeSettings.crossfadeDuration);
		}

		void FadeToLocal(string name){
			int t = FindSourceByName(name);
			if(t == -1) return;
			FadeToLocal(t, crossfadeSettings.crossfadeDuration);
		}

		void FadeToLocal(string name, float duration){
			int t = FindSourceByName(name);
			if(t == -1) return;
			FadeToLocal(t, duration);
		}

		public static void FadeTo(int index){
			Instance.FadeToLocal(index);
		}

		public static void FadeTo(int index, float duration){
			Instance.FadeToLocal(index, duration);
		}

		public static void FadeTo(string name){
			Instance.FadeToLocal(name);
		}

		public static void FadeTo(string name, float duration){
			Instance.FadeToLocal(name, duration);
		}

		// CROSSFADE BETWEEN ACTIVE AND CUED TRACKS
		void CrossfadeLocal(){
			FadeToLocal(fadingIndex, crossfadeSettings.crossfadeDuration);
		}

		public static void Crossfade(){
			Instance.CrossfadeLocal();
		}

		// PLAY --------------------------------------------------------------------------------------------------

		void Play(int index){
			if(!CheckIndexRange(index)) return;

			fadingName = playingName;
			fadingIndex = playingIndex;

			SwitchSources();
			MuteInactive();

			_xsrcTrack = _srcTrack;

			firstActive = !firstActive;

			playingName = sources[index].Name;
			playingIndex = index;
			_srcTrack = sources[index];

			_src.clip = sources[index].source;
			if(!_src.isPlaying) _src.Play();
			else _src.UnPause();
			_xsrc.Stop();

			isPlaying = true;
		}

		// Cues index1 and begins playing index0
		void Play(int index0, int index1){
			Play(index0);
			CueLocal(index1);
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
			Play(name0);
			CueLocal(name1);
		}

		public static void PlayByName(string name0, string name1){
			Instance.Play(name0, name1);
		}

		// CUE, PAUSE, STOP, EXIT LOOP ------------------------------------------------------------------------------------

		void CueLocal(int index){
			if(!CheckIndexRange(index)) return;

			_xsrcTrack = sources[index];
			fadingIndex = index;
			fadingName = _xsrcTrack.Name;

			_xsrc.Stop();
			_xsrc.clip = _xsrcTrack.source;
		}

		void CueLocal(string name){
			int t = FindSourceByName(name);
			if(t == -1) return;
			CueLocal(t);
		}

		public static void Cue(int index){
			Instance.CueLocal(index);
		}

		public static void Cue(string name){
			Instance.CueLocal(name);
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

		bool CheckIndexRange(int index){
			if(sources.Count < index + 1) return false;
			if(sources[index] == null) return false;

			return true;
		}

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

		// GETTERS / SETTERS ----------------------------------------------------------------------------

		string[] ListNames(){
			List<string> names = new List<string>();
			for(int i = 0; i < sources.Count; i++){
				names.Add(sources[i].Name);
			}
			return names.ToArray();
		}

		string[] ListDescNames(){
			List<string> names = new List<string>();
			for(int i = 0; i < sources.Count; i++){
				names.Add(sources[i].descName);
			}
			return names.ToArray();
		}

		public static string[] ListTracks(){
			return Instance.ListNames();
		}

		public static string[] ListTracksDesc(){
			return Instance.ListDescNames();
		}

		BGMTrackInfo[] ListInfo(){
			List<BGMTrackInfo> info = new List<BGMTrackInfo>();
			for(int i = 0; i < sources.Count; i++){
				info.Add(sources[i].GetTrackInfo());
			}
			return info.ToArray();
		}

		public static BGMTrackInfo[] ListTrackInfo(){
			return Instance.ListInfo();
		}

	}
}