using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (FollowPlayer))]
public class AttackIfClose : MonoBehaviour {

	public float attackDist = 0.5f;
	public string attackName = "attack";
	public bool aimToPlayer = true;
	FollowPlayer followai;
	public float attackChance = 1.0f;
	public float minInterval = 2.0f;
	bool inRange = false;
	float sinceLastAttack;

	Movement movt;
	// Use this for initialization
	void Start () {
		followai = GetComponent<FollowPlayer> ();
		movt = GetComponent<Movement> ();
		inRange = false;
	}
	void OnDestroy() {}
	
	// Update is called once per frame
	void Update () {
		if (followai.targetSet && movt.canMove) {	
			if (Vector3.Distance (followai.followObj.transform.position, transform.position) < attackDist) {
				if (sinceLastAttack > minInterval) {
					tryAttack ();
				}
			}
		} else {
			inRange = false;
		}
		sinceLastAttack += Time.deltaTime;
	}

	public void tryAttack () {
		if (Random.Range(0.0f,1.0f) <= attackChance) {
			if (aimToPlayer && followai) {
				followai.moveToPlayer ();
			}
			gameObject.GetComponent<Fighter> ().tryAttack (attackName);
			sinceLastAttack = 0.0f;
		}
	}
}
