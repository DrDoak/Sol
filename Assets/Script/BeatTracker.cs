using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeatTracker : MonoBehaviour {

	[System.Serializable]
	public class OnBeatEventHandler : UnityEngine.Events.UnityEvent
	{

	}
	public float Tempo;
	public float timePassed = 0f;
	public float offset = 0f;
	public float QuarterNoteInterval;
	float EigthNote;
	float SixteenthNote;
	public float LastTime = 0.0f;
	public float currTime = 0.0f;
	public float QuarterNoteActionTime = 0.0f;
	public OnBeatEventHandler onBeat;
	public int beatNo;
	public bool nextFrame;
	Image beatImg;
	List<Beats> allBeats = new List<Beats>();

	public AudioClip Music;
	private AudioSource GameMusic = null;

	void Awake(){
		/*QuarterNoteInterval = Tempo / 60.0f / 4.0f;*/
		beatNo = 0;
		QuarterNoteInterval = Tempo / 60.0f / 4.0f;
		EigthNote = QuarterNoteInterval / 4.0f;
		SixteenthNote = EigthNote / 4.0f;
		beatImg = GetComponent<Image> ();
		currTime = Time.time;


	}
	// Use this for initialization
	void Start () {
		
	}

	public void addBeatObj(Beats obj) {
		allBeats.Add (obj);
	}
	public void removeBeatObj(Beats obj){
		allBeats.Remove (obj);
	}
	
	// Update is called once per frame
	void Update () {
		if (GameMusic == null) {
			GameMusic = gameObject.AddComponent<AudioSource> ();
			GameMusic.clip = Music;
			GameMusic.loop = true;
			GameMusic.Play ();
			currTime = Time.time;
			Debug.Log (currTime);
		}
		
		float diff = QuarterNoteActionTime - (currTime + offset);
		if (diff <= Time.deltaTime / 2.0f || nextFrame) {
			
			beatNo = 1 + ((int)(QuarterNoteActionTime * 2) + 1) % 4;
			QuarterNoteActionTime += QuarterNoteInterval;


			if (beatImg != null) {
				switch (beatNo) {
				case 1:
					beatImg.color = Color.red;
					break;
				case 2:
					beatImg.color = Color.blue;
					break;
				case 3:
					beatImg.color = Color.yellow;
					break;
				default:
					beatImg.color = Color.green;
					break;
				}
			}
	
			foreach (Beats b in allBeats) {
				b.onBeat (beatNo);
			}
			//BroadcastMessage ("reactToBeat", beatNo);
			//Debug.Log (beatNo);
			nextFrame = false;
		} else if (diff <= Time.deltaTime) {
			nextFrame = true;
		}

		currTime = Time.time;
	}
}
