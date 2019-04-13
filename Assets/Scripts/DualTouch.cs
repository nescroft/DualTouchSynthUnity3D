using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class DualTouch : MonoBehaviour {

	public AudioSource[] asrc = new AudioSource[16];
	private double nextEventTime16;
	private float bpm = 120;
	private int x = 0;
	private bool running = false;
	private bool mute = false;
	private bool isSched = false;
	public bool[] arr = new bool[16];
	public AudioClip[] clip = new AudioClip[10];
	public MeshCreator mc;
	public Dropdown scaleDrop;
	public Dropdown noteDrop;
	public AudioMixerGroup audMixer;
	public GameObject handle;

	void Awake(){
		for (int i = 0; i < 16; i++) {
			asrc[i] = gameObject.AddComponent<AudioSource> ();
			asrc [i].outputAudioMixerGroup = audMixer;
			asrc [i].clip = clip[0];
		}
		running = true;
	}

	public void ChangeFxX(float x){
		x = -10000.0f * (1.0f-x);
		audMixer.audioMixer.SetFloat ("ReverbLevel", x);
	}

	public void ChangeFxY(float x){
		x = 22000.0f * x;
		if (x < 10) {
			x = 10;
		}
		audMixer.audioMixer.SetFloat ("HighCutoff", x);
	}


	public void ChangeSound(int x){
		for (int i = 0; i < 16; i++) {
			asrc [i].clip = clip[x];
		}
	}

	public void EffectEnter(BaseEventData ped) {
		PointerEventData pointerData = ped as PointerEventData;
		Vector2 localCursor;
		if (!RectTransformUtility.ScreenPointToLocalPointInRectangle (GetComponent<RectTransform> (), pointerData.position, pointerData.pressEventCamera, out localCursor)) {
			return;
		}
		float x = (localCursor.x- 450.0f);
		float y = (localCursor.y + 100);
		handle.GetComponent<RectTransform> ().localPosition = new Vector2 (x, y);
		x = (localCursor.x+340.0f)/680.0f;
		y = (localCursor.y+340.0f)/680.0f;
		ChangeFxX (x);
		ChangeFxY (y);
		Debug.Log("LocalCursor:" + x + " " + y);
	}

	void Start(){
		nextEventTime16 = AudioSettings.dspTime + 0.4F;
	}

	void FixedUpdate() {
		if (!running) {
			return;
		}
		double time = AudioSettings.dspTime;
		if (time + 0.2F > nextEventTime16) {
			if (!mute) {
				if (arr [x]) {
					//StartCoroutine (On (x, 0.198f));
					asrc [x].volume = 1.0f;
					asrc[x].PlayScheduled (nextEventTime16);
					double nextNoteTime = nextEventTime16;
					int arrWalkThru = x;
					for (int i = 0; i < 16; i++) {
						nextNoteTime += 30.0F / bpm;
						arrWalkThru++;
						if (arrWalkThru == 16) {
							arrWalkThru = 0;
						}
						if (arr [arrWalkThru]) {
							StartCoroutine (Off (x, (float)nextNoteTime - 0.1f));
							asrc [x].SetScheduledEndTime (nextNoteTime);
							i = (x+16);
						} 
					}
				}
			}
			nextEventTime16 += 30.0F / bpm;
			if (x > 14) {
				x = 0;
			} else {				
				x++; 
			}
		} 
	}

	IEnumerator On(int x, float time){
		yield return new WaitForSeconds (time);
		//asrc.pitch = patList [p].noteArr [i, j].pitch;
		asrc[x].volume = Random.Range(8.0f, 1.0f);
		//asrc.panStereo = patList [p].noteArr [i, j].pan;
	}

	IEnumerator Off(int x, float time){
		yield return new WaitForSeconds (time);
		asrc [x].volume = 0.0f;
	}

	public void Pattern(int num){
		num += 3;
		for (int i = 0; i < 16; i++) {
			arr [i] = false;
		}
		int j = 0;
		while (j < num) {
			int r = Random.Range (0, 16);
			if(!arr[r]){
			    arr [r] = true;
				j++;
			}
		}
	}

	public void Pitch(int num){
		float pitchVal = Mathf.Pow (1.05946f, noteDrop.value * (-1.0f));
		if (scaleDrop.value == 0) { //major
			if (num == 0) {
				pitchVal *= Mathf.Pow (1.05946f, 0);
			} else if (num == 1) {
				pitchVal *= Mathf.Pow (1.05946f, 2);
			} else if (num == 2) {
				pitchVal *= Mathf.Pow (1.05946f, 4);
			} else if (num == 3) {
				pitchVal *= Mathf.Pow (1.05946f, 5);
			} else if (num == 4) {
				pitchVal *= Mathf.Pow (1.05946f, 7);
			} else if (num == 5) {
				pitchVal *= Mathf.Pow (1.05946f, 9);
			} else if (num == 6) {
				pitchVal *= Mathf.Pow (1.05946f, 11);
			} else if (num == 7) {
				pitchVal *= Mathf.Pow (1.05946f, 12);
			} else if (num == 8) {
				pitchVal *= Mathf.Pow (1.05946f, 14);
			} else if (num == 9) {
				pitchVal *= Mathf.Pow (1.05946f, 16);
			} else if (num == 10) {
				pitchVal *= Mathf.Pow (1.05946f, 17);
			} else if (num == 11) {
				pitchVal *= Mathf.Pow (1.05946f, 19);
			} else if (num == 12) {
				pitchVal *= Mathf.Pow (1.05946f, 21);
			} else if (num == 13) {
				pitchVal *= Mathf.Pow (1.05946f, 23);
			}
	    } else if (scaleDrop.value == 1) { //minor
			if (num == 0) {
				pitchVal *= Mathf.Pow (1.05946f, 0);
			} else if (num == 1) {
				pitchVal *= Mathf.Pow (1.05946f, 2);
			} else if (num == 2) {
				pitchVal *= Mathf.Pow (1.05946f, 3);
			} else if (num == 3) {
				pitchVal *= Mathf.Pow (1.05946f, 5);
			} else if (num == 4) {
				pitchVal *= Mathf.Pow (1.05946f, 7);
			} else if (num == 5) {
				pitchVal *= Mathf.Pow (1.05946f, 8);
			} else if (num == 6) {
				pitchVal *= Mathf.Pow (1.05946f, 10);
			} else if (num == 7) {
				pitchVal *= Mathf.Pow (1.05946f, 12);
			} else if (num == 8) {
				pitchVal *= Mathf.Pow (1.05946f, 14);
			} else if (num == 9) {
				pitchVal *= Mathf.Pow (1.05946f, 15);
			} else if (num == 10) {
				pitchVal *= Mathf.Pow (1.05946f, 17);
			} else if (num == 11) {
				pitchVal *= Mathf.Pow (1.05946f, 19);
			} else if (num == 12) {
				pitchVal *= Mathf.Pow (1.05946f, 20);
			} else if (num == 13) {
				pitchVal *= Mathf.Pow (1.05946f, 22);
			} 
		}
		mc.GetComponent<MeshRenderer> ().material.color = Color.HSVToRGB (num/14.0f, 0.5f, 1.0f);
	    for (int i = 0; i < asrc.Length; i++) {
	    	asrc [i].pitch = pitchVal;
    	}

	}

	public void Tempo(float x){
		bpm = (int)x;
		mc.rotationX = x*0.25f;
	}
}
