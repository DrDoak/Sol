using UnityEngine;
using System.Collections;

public class disappearing : MonoBehaviour {

	public float duration = 3.0f;
	public bool toDisappear = true;

	public AudioClip TickingSound;
	// Use this for initialization
	void Start () {
		if (TickingSound != null) {AudioSource.PlayClipAtPoint (TickingSound, gameObject.transform.position);}
	}

	// Update is called once per frame
	void Update () {
		if (duration > 0.0f || !toDisappear) {
			duration = duration - Time.deltaTime;
		} else {
			GameObject.Destroy (gameObject);
		}
	}
	void setDuration (float time) {
		duration = time;
	}
}
