using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtkChain : AtkDash {

	public string chainAttack = "attack";
	public float changeRecoveryOnhit = 0.0f;
	public float minRecovery = 0.0f;
	public bool onlyOnHit = false;
	public string playerKey = "auto";
	public float cancelTolerance = 0.15f;
	bool hit;
	bool conclude = false;

	void Start () {}
	// Update is called once per frame
	void Update () {}


	public override void onAttack() {
		base.onAttack ();
		conclude = false;
	}

	public override void recoveryTick() {
		if (GetComponent<Player>() && timeSinceStart < (recoveryTime + startUpTime - cancelTolerance)) {
			if (playerKey != "auto" && Input.GetKeyDown (playerKey)) {
				tryNewAttack ();
			}
		}
	}
	void tryNewAttack() {
		if (!conclude && (!onlyOnHit || hit)) {
			conclude = true;
			GetComponent<Fighter> ().endAttack ();
			GetComponent<Fighter> ().tryAttack (chainAttack);
		}
	}

	public override void onConclude() {
		if (!GetComponent<Player> () || playerKey == "auto") {
			tryNewAttack ();
		}
	}

	public override void onHitConfirm(GameObject other) {
		if (other.GetComponent<Attackable> ()) {
			hit = true;
		}
		GetComponent<Fighter>().recoveryTime = Mathf.Max(minRecovery,GetComponent<Fighter>().recoveryTime + changeRecoveryOnhit);
	}
}
