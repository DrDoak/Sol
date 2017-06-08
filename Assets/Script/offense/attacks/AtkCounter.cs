using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtkCounter : AttackInfo {

	public string resistanceType = "melee";
	public float resistanceTime = 1.0f;
	public bool counterAttack;
	public string counterAttackType = "attack";

	// Use this for initialization
	void Start () {
		attackable = GetComponent<Attackable> ();
		fighter = GetComponent<Fighter> ();
	}
		
	// Update is called once per frame
	void Update () {}

	public override void onAttack() {
//		Debug.Log ("adding resistance");
		attackable.addResistence (resistanceType, resistanceTime);
	}

	public override void onInterrupt(float stunTime, bool successfulHit, hitbox hb) {
//		Debug.Log ("on Interrupt");
		if (counterAttack && !successfulHit && timeSinceStart > startUpTime && timeSinceStart < (startUpTime + resistanceTime)) {
			fighter.endAttack ();
			fighter.tryAttack (counterAttackType);
		}
	}
}
