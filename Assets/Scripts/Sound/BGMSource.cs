using UnityEngine;
using System.Collections;

namespace Softdrink{
	public enum BGMLoopMode{
		None,
		LoopEntire,
		LoopPointFromStart,
		LoopPointFromEnd,
	}
	// A ScriptableObejct designed to store information about BGM songs, etc. for playback and looping
	// Designed to be flexible and modular
	[CreateAssetMenu(menuName = "Sound / BGM Source")]
	public class BGMSource : ScriptableObject {

		[TooltipAttribute("The String name that will be used to search for and reference this BGM Source.")]
		public string name = "New BGM Source";

		[TooltipAttribute("The AudioClip that this BGM Source reads from. \nShould mostly be set to either Compressed In Memory or Streaming Compression Mode.")]
		public AudioClip source = null;

		[TooltipAttribute("The Loop Mode of this BGM Source. \nNone - This BGM source does not loop. \nLoop Entire - Loop the entire file, end to end. \nLoop Point From Start - The BGM will loop, and resume 'Loop Time' seconds from the Start of file. \nLoop Point From End - The BGM will loop, and resume 'Loop Time' seconds from the end of file.")]
		public BGMLoopMode loopMode = BGMLoopMode.None;

		[TooltipAttribute("The Loop Time. Defines at what point the file should resume play after a Loop.")]
		public float loopTime = 0f;
		
	}
}
