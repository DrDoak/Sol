using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Relationship {
	public Character ParentChar;
	public string Name;
	//Relative ability:
	public float relativeCombat = 0.0f;
	public float relativeLogic = 0.0f;
	public float relativeSocial = 0.0f;

	//Power
	public float authority = 0.0f;
	public float affirmation = 0.0f;

	//Personal
	public float favorability = 0.0f;
	public float relevance = 0.0f;
	public bool openHostile = false;

	//Knowledge based
	public float lastTimeSeen = 0.0f;
	public Vector3 lastPosition;
	public string lastRoom;
	public string knownFaction;
	public bool canSee;

	Personality pers;
/*	//Mood
	public float confidence = 0.0f;
	public float happiness = 0.0f;
	public float emotion = 0.0f;
*/
	public void ChangeFavor(float favorChange, bool changeRelevance = true, bool paradigmShift = false) {
		if (paradigmShift) {
			favorability += favorChange;
		} else {
			favorability += favorChange * (1f - relevance);
		}	
		//Debug.Log ("Favor changed by " + favorChange * (1f - relevance) + " to: " + favorability);
		if (changeRelevance) {
		}
	}
	public float GetFavorScaled() {
		return (favorability * (2f - relevance)/2f) * (1f + ParentChar.PersonalityData.opennessAllegiance);
	}

	public float GetAuthorityScaled() {
		Personality p = ParentChar.PersonalityData;
		return GetFavorScaled() * Mathf.Max(0f,-p.emotionLogic) + authority * (2f - affirmation) * (1f + ParentChar.PersonalityData.opennessAllegiance) + p.agreeableness;
	}
}
