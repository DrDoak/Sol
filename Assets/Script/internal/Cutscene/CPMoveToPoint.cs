using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPMoveToPoint : CutscenePiece {
	//public NPC targetNPC;
	public Vector3 targetPoint;
	public float proximity = 0.1f;
	Character targetNPC;
	Color debugColor = Color.blue;
	// Use this for initialization
	void Start () {
		init ();
	}
	void Update () {}
	public override void onEventStart() {
		targetNPC = cm.findChar (targetCharName);
		if (targetNPC.GetComponent<Player> ()) {
			targetNPC.GetComponent<Player> ().setTargetPoint (targetPoint, proximity);
		} else {
			targetNPC.GetComponent<NPCMovement> ().setTargetPoint (targetPoint, proximity);
		}
	}
	public override void activeTick(float dt) {
		Vector3 pos = targetNPC.transform.position;
		float weightedD = Mathf.Sqrt (Mathf.Pow (targetPoint.x - pos.x, 2) + Mathf.Pow (targetPoint.y - pos.y, 2) * 0.15f);
		Debug.Log (weightedD);
		if (weightedD < proximity) {
			parent.progressEvent ();
		}
	}
	void OnDrawGizmos() {
		Gizmos.color = debugColor;
		Gizmos.DrawCube (targetPoint,new Vector3(1,1,1));
	}
}
