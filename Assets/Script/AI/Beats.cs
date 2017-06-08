using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beats : MonoBehaviour {

	// Use this for initialization
	void Start () {}

	protected void init() {
//		Debug.Log ("start beats");
		FindObjectOfType<BeatTracker> ().addBeatObj (this);
	}
	protected void dest() {
//		Debug.Log ("destroy beats");
		//FindObjectOfType<BeatTracker> ().removeBeatObj (this);
	}
	
	// Update is called once per frame
	void Update () {}

	public virtual void onBeat (int beatNo) {}
}
