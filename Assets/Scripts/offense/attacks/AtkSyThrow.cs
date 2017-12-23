using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtkSyThrow : AtkDash {

	public List<Vector2> Targets;
	public float Delay = 0.1f;
	SylviaOffense m_SyOffense;

	void Start () {	
		m_SyOffense = GetComponent<SylviaOffense> ();
	}
	void Update () { }

	public override void onAttack() {
		if (!movement.facingLeft) {
			movement.addSelfForce (attackDash, attackDashDuration);
		} else {
			movement.addSelfForce (new Vector2 (-1f * attackDash.x, attackDash.y), attackDashDuration);
		}

		List<Vector2> newTargets = new List<Vector2> ();
		bool fL = GetComponent<Movement> ().facingLeft;
		foreach (Vector2 v2 in Targets) {
			Vector3 newPos = new Vector3 (transform.position.x, transform.position.y + v2.y, transform.position.z);
			if (fL) {
				newPos.x -= v2.x;
			} else {
				newPos.x += v2.x;
			}
			newTargets.Add (newPos);
		}
		m_SyOffense.TargetPoint (newTargets,Delay);
	}
}
