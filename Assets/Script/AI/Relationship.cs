using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Relationship {
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

	Personality pers;
	/*
	 * //Character Traits:
	public float egoCombat = 0.0f;
	public float egoLogic = 0.0f;
	public float egoSocial = 0.0f;

	public float boldness = 0.0f;
	public float temperament = 0.5f;
	public float emotionLogic = 0.0f;
	public float opennessAllegiance = 0.0f;
	public float agreeableness = 0.0f;
	public float pragmaticIdealistic = 0.0f;

	//Mood
	public float confidence = 0.0f;
	public float happiness = 0.0f;
	public float emotion = 0.0f;
*/

}
