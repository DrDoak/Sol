using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPMoveToPoint : CutscenePiece {
	public NPC targetNPC;
	public Vector3 targetPoint;
	public float proximity;
	// Use this for initialization
	void Start () {}
	void Update () {}
	public override void onEventStart() {
		targetNPC.GetComponent<NPCMovement> ().setTargetPoint (targetPoint, proximity);
	}
	public override void activeTick(float dt) {
		if (Vector3.Distance (targetNPC.transform.position, targetPoint) < proximity) {
			parent.progressEvent ();
		}
	}
}
