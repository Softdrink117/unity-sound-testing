using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Softdrink{
	[AddComponentMenu("Scripts/Sound/Testers/SFX Player Tester")]
	public class SFX_Player_Tester : MonoBehaviour {

		private SFXPlayer _player;

		void Awake(){
			_player = gameObject.GetComponent<SFXPlayer>() as SFXPlayer;
		}

		void Update(){
			if(Input.GetMouseButtonDown(0))_player.Play();
			if(Input.GetMouseButtonDown(1))_player.PlayOneShot();
			if(Input.GetKeyDown(KeyCode.Escape)) _player.HardStop();
			if(Input.GetKeyDown(KeyCode.Space)) _player.Stop();
		}
	}
}