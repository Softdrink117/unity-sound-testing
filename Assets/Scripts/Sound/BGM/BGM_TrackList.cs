using UnityEngine;
using System.Collections;

namespace Softdrink{
	[AddComponentMenu("Scripts/Sound/BGM Tracklist")]
	public class BGM_TrackList : MonoBehaviour {

		[TooltipAttribute("A list of all BGMTrackInfo structs from the BGM_Manager.")]
		public BGMTrackInfo[] trackInfo;

		void Start(){
			trackInfo = BGM_Manager.ListTrackInfo();
		}
	}
}