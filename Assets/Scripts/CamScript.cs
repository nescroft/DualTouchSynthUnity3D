using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamScript : MonoBehaviour {
	public MeshCreator mc;
	public void OnAudioFilterRead(float[] data, int channels){ //this function runs repeatedly during audiolistener recording
		for (int i = 0; i < data.Length; i++) { 	
			float x = 6.0f - Mathf.Abs (data [i] * 32767 * 0.0002f);
			if (x < 0) {
				x = 0.1f;
			} 
			mc.RoundnessSlider (x);
		}   
	}
}
