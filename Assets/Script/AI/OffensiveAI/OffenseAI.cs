using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Fighter))]
[RequireComponent (typeof (NPCMovement))]
public class OffenseAI : MonoBehaviour {

	public List<AttackInfo> allAttacks;
	AttackInfo currentAttack;
	public Character currentTarget;

	public float baseSpacing = 1.0f;
	public float baseReactionSpeed = 1.0f;
	public float baseDecisionMaking = 1.0f;
	public float baseAggression = 0.5f;

	float spacing;
	float reactionSpeed;
	float decisionMaking;
	float aggression;
	Fighter mF;
	NPCMovement npcM;

	string currentAction = "wait";

	void Start () {
		spacing = baseSpacing;
		reactionSpeed = baseReactionSpeed;
		decisionMaking = baseDecisionMaking;
		aggression = baseAggression;
		allAttacks = new List<AttackInfo>(GetComponents<AttackInfo> ());
		mF = GetComponent<Fighter> ();
		npcM = GetComponent<NPCMovement> ();
	}

	void Update () {
		if (currentTarget != null) {
			if (currentAction == "wait") {
				decideNextAction ();
			} else if (currentAction == "moveToTarget") {
				npcM.moveToPoint (currentTarget.transform.position);
				decideNextAction ();
			} else if (currentAction == "attack") {
				if (mF.currentAttackName == "none") {
					decideNextAction ();
				}
			}
		}
	}

	public void decideNextAction() {
		Vector3 otherPos = currentTarget.transform.position;
		float xDiff = Mathf.Abs(transform.position.x - otherPos.x);
		float yDiff = Mathf.Abs(transform.position.y - otherPos.y);
		foreach (AttackInfo ainfo in allAttacks) {
			if ((ainfo.hitboxScale.x + ainfo.offset.x) + 
				(ainfo.hitboxScale.x + ainfo.offset.x) * Random.Range(0f, 1f-spacing) > xDiff &&
				(ainfo.hitboxScale.y + ainfo.offset.y) + 
				(ainfo.hitboxScale.y + ainfo.offset.y) * Random.Range(0f, 1f-spacing)> yDiff) {
				mF.tryAttack (ainfo.attackName);
				currentAction = "attack";
				break;
			}
		}
		currentAction = "moveToTarget";
	}

	public void commitToAction() {}

	public void setTarget(Character c) {
		Debug.Log ("set target to: " + c);
		currentTarget = c;		
	}
}
