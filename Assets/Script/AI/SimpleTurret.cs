using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent (typeof (Beats))]
public class SimpleTurret : MonoBehaviour {

	public float interval = 3.0f;
	public float currentInt;
	Beats b;
	// Use this for initialization
	void Start () {
		currentInt = interval;

		b = GetComponent<Beats> ();
	}

	// Update is called once per frame
	void Update () {
		
		currentInt = Mathf.Max (0.0f, currentInt - Time.deltaTime);
		if (currentInt <= 0.0f) {
//			Debug.Log ("Beat");
			b.onBeat (1);
			currentInt = interval;
		}
	}
}
