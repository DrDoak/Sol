using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Shooter))]
public class AtkShoot : AttackInfo {
	Shooter mShoot;
	public GameObject projectile;
	public Vector2 shotOffset = new Vector2 (0.0f, 0.0f);
	Movement movt;
	// Use this for initialization
	void Start () {
		mShoot = GetComponent<Shooter> ();
		movt = GetComponent<Movement> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override void onAttack () {
		mShoot.fire (shotOffset,movt.facingLeft,GetComponent<Attackable>().faction,projectile);
	}
}
