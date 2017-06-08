using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hitboxMulti : hitbox {

	public float refreshTime = 0.1f;
	public float timeSinceLast = 0.0f;
	// Use this for initialization
	void Start () {}

	void Update () {
		timeSinceLast += Time.deltaTime;
		if (timeSinceLast > refreshTime) {
			timeSinceLast = 0.0f;
			collidedObjs.Clear ();
			foreach (Attackable cont in overlappingControl) {
				onAttackable (cont);
			}
		}		
		base.updateTick ();
	}
}
