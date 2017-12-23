using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

namespace Softdrink{
	[CreateAssetMenu(menuName = "Sound/Volume Manager Settings")]
	public class VolumeManagerSettings : ScriptableObject {

		[TooltipAttribute("The target AudioMixer.")]
		public AudioMixer mixer = null;

		[TooltipAttribute("The string name of the exposed parameter of the AudioMixer for Master Volume.")]
		public string masterParameter = "";

		[TooltipAttribute("The string name of the exposed parameter of the AudioMixer for Master Volume.")]
		public string musicParameter = "";

		[TooltipAttribute("The string name of the exposed parameter of the AudioMixer for Master Volume.")]
		public string sfxParameter = "";

		[TooltipAttribute("The string name of the exposed parameter of the AudioMixer for Master Volume.")]
		public string voiceParameter = "";
		
	}
}