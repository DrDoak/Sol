using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtkItem : AttackInfo {

	public GameObject item;
	public Vector2 itemOffset = new Vector2 (0.0f, 0.0f);
	public Vector2 initialVelocity = new Vector2 (0.0f, 0.0f);
	public float velocityTime;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override void onAttack() {
		bool fL = GetComponent<Movement>().facingLeft;
		Vector3 newPos;
		if (fL) {
			newPos = new Vector3(transform.position.x - itemOffset.x, transform.position.y + itemOffset.y, 0);
		} else {
			newPos = new Vector3(transform.position.x + itemOffset.x, transform.position.y + itemOffset.y, 0);
		}
		GameObject go = Instantiate(item,newPos,Quaternion.identity) as GameObject; 


		if (go.GetComponent<Movement>()) {
			Vector2 realVel = initialVelocity;
			if (fL) {
				realVel.x = -initialVelocity.x;
			}
			Movement movt = go.GetComponent<Movement> ();
			if (velocityTime > 0.0f) {
				movt.addSelfForce(realVel,velocityTime);
			} else {
				movt.addToVelocity (realVel);
			}
			movt.facingLeft = fL;
		}
	}
}
