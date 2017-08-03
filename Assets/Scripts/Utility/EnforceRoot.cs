#if UNITY_EDITOR

using UnityEngine;
using System.Collections;

namespace Softdrink{
	// A very simple script that forces the connected GameObject
	// to *always* be a Root in the Scene Hierarchy.
	// This can be useful for things like Global Managers and
	// Singleton-type structures that persist between scenes;
	// it can prevent designers from accidentally breaking the
	// Hierarchy order.

	// There are probably ways to do this that are more optimized,
	// but this was the first solution that came to mind.
	[ExecuteInEditMode]
	public class EnforceRoot : MonoBehaviour {

		void Awake(){
			if(Application.isPlaying) Destroy(this);
		}

		void Update(){
			if(gameObject.transform.parent != null)
				gameObject.transform.parent = null;
		}
	}
}

#endif