using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Fighter))]
public class IntervalAttack : MonoBehaviour {

	public string attackName = "attack";
	public float chance = 1.0f;
	Fighter mFighter;
	//NPCMovement followai;
	public float minInterval = 5.0f;
	bool inRange = false;
	float sinceLastAttack;

	Movement movt;
	// Use this for initialization
	void Start () {
		mFighter = GetComponent<Fighter> ();
		movt = GetComponent<Movement> ();
	}
	void OnDestroy() {	}

	// Update is called once per frame
	void Update () {
		if (movt.canMove) {	
			if (sinceLastAttack > minInterval) {
				tryAttack ();
			}
		}
		sinceLastAttack += Time.deltaTime;
	}

	public void tryAttack () {
		if (Random.Range(0.0f,1.0f) <= chance) {
			mFighter.tryAttack (attackName);
		}
	}
}