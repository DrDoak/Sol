using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleTurret : MonoBehaviour {

	public float interval = 3.0f;
	public float currentInt;
	// Use this for initialization
	void Start () {
		currentInt = interval;
	}

	// Update is called once per frame
	void Update () {
		
		currentInt = Mathf.Max (0.0f, currentInt - Time.deltaTime);
		if (currentInt <= 0.0f) {
			currentInt = interval;
		}
	}
}
