using UnityEngine;
using System.Collections;

namespace Softdrink{
	[CreateAssetMenu(menuName = "Sound/ Crossfade Settings")]
	public class CrossfadeSettings : ScriptableObject {

		[HeaderAttribute("Crossfades")]

		[TooltipAttribute("Default duration of the Crossfade, in Seconds.")]
		public float crossfadeDuration = 1f;

		[TooltipAttribute("Gain curve for input edge.")]
		public AnimationCurve gain1 = new AnimationCurve(new Keyframe(0f,1f), new Keyframe(1f, 0f));

		[TooltipAttribute("Gain curve for output edge.")]
		public AnimationCurve gain2 = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

		[TooltipAttribute("When enabled, crossfaded songs will continue playing in the background after they've been faded to zero, instead of stopping.")]
		public bool continuePlayInBG = true;

		[HeaderAttribute("Fade In / Out")]

		[TooltipAttribute("Default duration of the Fade, in Seconds.")]
		public float fadeDuration = 1f;

		[TooltipAttribute("Gain curve for Fade In.")]
		public AnimationCurve fadeIn = new AnimationCurve(new Keyframe(0f,0f), new Keyframe(1f,1f));

		[TooltipAttribute("Gain curve for Fade Out.")]
		public AnimationCurve fadeOut = new AnimationCurve(new Keyframe(0f,1f), new Keyframe(1f,0f));
	}
}
