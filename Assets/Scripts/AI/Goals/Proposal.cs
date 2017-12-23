using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proposal {

	public NPC mNPC;
	public Event mEvent;
	public Goal.executionMethod mMethod;
	public Goal.executionMethod evalMethod;
	public float rating = 0.0f;
	public bool movement = false;

	public Proposal() {
		evalMethod = genericEvaluate;
	}
	// Use this for initialization
	public void setRating(float f) {
		rating = f;
	}
		
	public float getRating() {
		return rating;
	}
	
	// Update is called once per frame
	void Update () {}

	protected void resolve() {
		mNPC.resolveProposal (this);
	}
	void genericEvaluate(Proposal p) {}

}
