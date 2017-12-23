using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Softdrink{
	// Class to contain Sound Effect data
	[CreateAssetMenu(menuName = "Sound/SFX Source")]
	public class SFXSource : ScriptableObject {

		[TooltipAttribute("The name of this SFXSource. \nMostly used for human-readibility and tagging/meta purposes.")]
		public string Name = "New SFX Source";

		[TooltipAttribute("Audio clips to be used as sources. If more than one is included, one will be randomly selected each time the SFXSource is Played.")]
		public AudioClip[] sources;

		[TooltipAttribute("Playback settings for this SFXSource.")]
		public SFXPlaybackSettings settings;

	}
}