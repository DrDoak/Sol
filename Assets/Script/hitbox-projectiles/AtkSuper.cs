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
	public List<Attackable> collidedObjs = new List<Attackable> (); 
	bool successfulInterupt = false;
	Attackable attackable;

	// Use this for initialization
	void Start () {
		attackable = GetComponent<Attackable> ();
		Vector3 newPos = new Vector3(transform.position.x, transform.position.y, 0);
	}

	
	// Update is called once per frame
	void Update () {}
	public override void onStartUp() {
		base.onStartUp ();
		collidedObjs.Clear ();
		successfulInterupt = false;
		attackable.modifyEnergy (-20.0f);
	}

	public override void onHitConfirm(GameObject other) {
		if (!collidedObjs.Contains (other.GetComponent<Attackable> ())) {
			GameObject mH = Instantiate (multiHitbox, other.transform.position, Quaternion.identity);
			hitboxMulti newBox = mH.GetComponent<hitboxMulti> ();
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
			collidedObjs.Add (other.GetComponent<Attackable> ());
		}

	}
	public override void recoveryTick() {
		if (GetComponent<Player>() != null) {
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
			attackable.modifyEnergy (-100.0f);
		}
	}
}
