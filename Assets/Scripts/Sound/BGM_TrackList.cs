using UnityEngine;
using System.Collections;

namespace Softdrink{
	public class BGM_TrackList : MonoBehaviour {

		[TooltipAttribute("A list of all track names from the BGM_Manager.")]
		public string[] tracklist;

		[TooltipAttribute("A list of all descriptive names from the BGM_Manager.")]
		public string[] tracklistDesc;

		void Start(){
			tracklist = BGM_Manager.ListTracks();
			tracklistDesc = BGM_Manager.ListTracksDesc();
		}
	}
}