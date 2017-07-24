using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Softdrink{

	// Singleton manager that handles BGM stuff
	// Capable of fading in/out, looping, etc.
	// Can reference a playlist of BGM Source files

	// PERSISTS BETWEEN SCENES
	[RequireComponent(typeof(AudioSource))]
	public class BGM_Manager : MonoBehaviour {

		// Singleton Instance
		public static BGM_Manager Instance;

		[HeaderAttribute("Playback Status")]

		[TooltipAttribute("READONLY: The currently playing string name of the active BGM Source.")]
		public string playingName = "";

		[TooltipAttribute("READONLY: The currently playing index in the Sources list.")]
		public int playingIndex = 0;

		[TooltipAttribute("Is there any BGM playing?")]
		public bool isPlaying = false;

		[HeaderAttribute("Sources")]

		[TooltipAttribute("A list of all BGM Sources that this BGM Manager is currently referencing.")]
		public List<BGMSource> sources = new List<BGMSource>();

		[HeaderAttribute("Behvaior Options")]

		[TooltipAttribute("Should the BGM begin playing on Awake?")]
		public bool playOnAwake = true;

		[TooltipAttribute("The List index of the BGM to begin playing On Awake. \nNOTE: This should usually be zero!")]
		public int playOnAwakeIndex = 0;

		private AudioSource _src = null;

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
			_src = gameObject.GetComponent<AudioSource>() as AudioSource;
			if(_src == null) Debug.LogError("ERROR: The BGM_Manager could not associate a reference to the attached AudioSource component!", this);
		}

		void Play(int index){
			if(sources.Count < index + 1) return;
			if(sources[index] == null) return;

			playingName = sources[index].name;
			playingIndex = index;

			_src.clip = sources[index].source;
			_src.Play();
		}
		
		void Update(){
			// << SUPER DEBUG >>
			if(Input.GetKeyDown(KeyCode.Space)){
				playingIndex++;
				if(playingIndex >= sources.Count) playingIndex = 0;
				Play(playingIndex);
			}
		}
	}
}