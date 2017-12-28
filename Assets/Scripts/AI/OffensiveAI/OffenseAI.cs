﻿using System.Collections;
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
	Fighter m_fighter;
	NPCMovement npcM;

	public string currentAction = "wait";

	void Start () {
		spacing = baseSpacing;
		reactionSpeed = baseReactionSpeed;
		decisionMaking = baseDecisionMaking;
		aggression = baseAggression;
		allAttacks = new List<AttackInfo>(GetComponents<AttackInfo> ());
		m_fighter = GetComponent<Fighter> ();
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
				if (m_fighter.currentAttackName == "none") {
					decideNextAction ();
				}
			}
		}
	}

	public void decideNextAction() {
		Vector3 otherPos = currentTarget.transform.position;
		float xDiff = Mathf.Abs(transform.position.x - otherPos.x);
		float yDiff = Mathf.Abs(transform.position.y - otherPos.y);
		if (Random.value < (aggression * 0.1f)) {
			foreach (AttackInfo ainfo in allAttacks) {
				if ((ainfo.AIPredictionHitbox.x + ainfo.AIPredictionOffset.x) +
				   (ainfo.AIPredictionHitbox.x + ainfo.AIPredictionOffset.x) * Random.Range (0f, 1f - spacing) > xDiff &&
				   (ainfo.AIPredictionHitbox.y + ainfo.AIPredictionOffset.y) +
				   (ainfo.AIPredictionHitbox.y + ainfo.AIPredictionOffset.y) * Random.Range (0f, 1f - spacing) > yDiff) {
					m_fighter.tryAttack (ainfo.attackName);
					currentAction = "attack";
					break;
				}
			}
		}
		currentAction = "moveToTarget";
	}

	public void commitToAction() {}


	public void setTarget(Character c) {
		currentTarget = c;		
	}
}