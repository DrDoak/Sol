using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtkSuper : AtkDash {

	public GameObject multiHitbox;
	public float multiHitDuration;
	public float multiHitInterval;
	public Vector2 minKnockback;
	public Vector2 maxKnockback;
	public string playerKey = "l";
	List<Attackable> multiHitObjs = new List<Attackable> (); 
	bool successfulInterupt = false;
	Attackable atk;

	// Use this for initialization
	void Start () {
		atk = GetComponent<Attackable> ();
		init ();
	}

	
	// Update is called once per frame
	void Update () {}
	public override void onStartUp() {
		base.onStartUp ();
		multiHitObjs.Clear ();
		successfulInterupt = false;
		atk.modifyEnergy (-20.0f);
	}

	public override void onHitConfirm(GameObject other) {
		if (!multiHitObjs.Contains (other.GetComponent<Attackable> ())) {
			GameObject mH = Instantiate (multiHitbox, other.transform.position, Quaternion.identity);
			HitboxMulti newBox = mH.GetComponent<HitboxMulti> ();
			newBox.setDamage (damage);
			newBox.setHitboxDuration (multiHitDuration);
			newBox.randomizeKnockback (minKnockback.x, maxKnockback.x, minKnockback.x, maxKnockback.y);
			newBox.setFaction (GetComponent<Attackable> ().faction);
			newBox.setFollow (other, Vector2.zero);
			newBox.creator = gameObject;
			newBox.reflect = false;
			newBox.stun = stun;
			newBox.refreshTime = multiHitInterval;
			newBox.randomizeKnockback (minKnockback.x, maxKnockback.x, minKnockback.y, maxKnockback.y);
			multiHitObjs.Add (other.GetComponent<Attackable> ());
		}

	}
	public override void recoveryTick() {
		if (GetComponent<Playable>() != null) {
			if (Input.GetKeyDown (playerKey) && attackable.energy >= 20.0f) {
				successfulInterupt = true;
				if (Input.GetKey ("a")) {
					GetComponent<Movement> ().setFacingLeft( true);
				} else if (Input.GetKey("d")) {
					GetComponent<Movement> ().setFacingLeft( false);
				}
				GetComponent<Fighter> ().endAttack ();
				GetComponent<Fighter> ().tryAttack ("super");
			}
		}
	}
	public override void onConclude() {
		if (!successfulInterupt) {
			atk.modifyEnergy (-100.0f);
		}
	}
}
