using UnityEngine;
using System.Collections;

namespace Softdrink{
	public enum BGMLoopMode{
		None,
		LoopEntire,
		LoopFromStart,
		LoopToEnd,
		LoopFromStartToEnd,
	};

	// A ScriptableObejct designed to store information about BGM songs, etc. for playback and looping
	// Designed to be flexible and modular
	[CreateAssetMenu(menuName = "Sound/ BGM Source")]
	public class BGMSource : ScriptableObject {

		[TooltipAttribute("The String name that will be used to search for and reference this BGM Source.")]
		public string Name = "New BGM Source";

		[TooltipAttribute("The Descriptive Name that will be exposed to the Player or in sound test menus. This might be the actual song or track title, for example.")]
		public string descName = "Track Title";

		[TooltipAttribute("An optional Description for this BGMSource. This could be used for a Sound Test menu, for example.")]
		[MultilineAttribute(5)]
		public string description = "";

		[TooltipAttribute("The AudioClip that this BGM Source reads from. \nShould mostly be set to either Compressed In Memory or Streaming Compression Mode.")]
		public AudioClip source = null;

		[TooltipAttribute("The Loop Mode of this BGM Source. \nNone - This BGM source does not loop. \nLoop Entire - Loop the entire file, end to end. \nLoop From Start - begin a loop from the Start Time specified below. \nLoop To End - play until the End Time specified below, then begin a loop. \nLoop From Start To End - Play, then loop starting from the Start Point until the End Point specified below.")]
		public BGMLoopMode loopMode = BGMLoopMode.None;

		[TooltipAttribute("The Loop Start Time (s). Defines at what point the file should resume play after a Loop.")]
		public float loopStartTime = 0f;

		[TooltipAttribute("The Loop End Time (s). Defines at what point the file should begin a Loop.")]
		public float loopEndTime = 0f;



		#if UNITY_EDITOR
		void OnValidate(){
			Validate();
		}
		#endif

		public void Validate(){
			if(loopEndTime < loopStartTime) loopEndTime = loopStartTime + 0.001f;
		}	
		
	}
}
