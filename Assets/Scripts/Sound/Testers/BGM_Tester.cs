using UnityEngine;
using System.Collections;

namespace Softdrink{
	[AddComponentMenu("Scripts/Sound/Testers/BGM Manager Tester")]
	public class BGM_Tester : MonoBehaviour {

		[SerializeField]
		[TooltipAttribute("The String name of a track to play.")]
		private string playName = "";

		[SerializeField]
		[TooltipAttribute("The integer index number of a track to play.")]
		private int playIndex = 0;

		void Start () {
		}

		void OnValidate(){
			BGM_Manager _bm = null;
			_bm = GameObject.Find("BGM_Manager").GetComponent<BGM_Manager>() as BGM_Manager;
			if(_bm == null) Debug.LogError("ERROR! BGM_Tester could not find the BGM_Manager in the scene!", this);

			if(_bm == null) return;

			if(playIndex >= _bm.sources.Count) playIndex = _bm.sources.Count - 1;
			if(playIndex < 0) playIndex = 0;
		}

		[ContextMenu("Play Name")]
		public void PlayName(){
			// Debug.Log("PlayName", this);
			BGM_Manager.PlayByName(playName);
		}

		[ContextMenu("Play Index")]
		public void PlayIndex(){
			// Debug.Log("PlayIndex", this);
			BGM_Manager.PlayByIndex(playIndex);
		}

		// >>> THIS ISN'T WORKING FOR SOME REASON!

		[ContextMenu("Crossfade")]
		public void Crossfade(){
			// Debug.Log("Crossfade", this);
			BGM_Manager.Crossfade();
		}
		
	}
}