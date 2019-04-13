using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderText : MonoBehaviour {

	public Text handleText;

	public void UpdateText(float val){
		handleText.text = ""+val+" BPM";
	}
}
