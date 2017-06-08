using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtkDash : AttackInfo {

	public Vector2 startUpDash = new Vector2 (0.0f, 0f);
	public float startUpDuration = 0.0f;
	public Vector2 attackDash = new Vector2 (0.0f, 0f);
	public float attackDashDuration = 0.0f;
	public Vector2 conclusionDash = new Vector2 (0.0f, 0f);
	public float conclusionDuration = 0.0f;
	Movement movement;
	// Use this for initialization
	void Start () {
		
	}	
	// Update is called once per frame
	void Update () {}

	public override void onStartUp() {
		movement = GetComponent<Movement> ();
		if (!movement.facingLeft) {
			movement.addSelfForce (startUpDash, startUpDuration);
		} else {
			movement.addSelfForce (new Vector2(-1f *startUpDash.x,startUpDash.y), startUpDuration);
		}
	}
	public override void onAttack() {
		if (!movement.facingLeft) {
			movement.addSelfForce (attackDash, attackDashDuration);
		} else {
			movement.addSelfForce (new Vector2 (-1f * attackDash.x, attackDash.y), attackDashDuration);
		}
	}
	public override void onConclude() {
		if (!movement.facingLeft) {
			movement.addSelfForce (conclusionDash, conclusionDuration);
		} else {
			movement.addSelfForce (new Vector2 (-1f * conclusionDash.x, conclusionDash.y), conclusionDuration);
		}
	}
}
