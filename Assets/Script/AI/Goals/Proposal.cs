using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proposal : MonoBehaviour {

	public delegate void proposedMethod(Proposal p);
	public NPC mNPC;
	public Event mEvent;
	public proposedMethod mMethod;
	public proposedMethod evalMethod;
	public float rating;
	public bool movement = false;
	// Use this for initialization
	void Start () {
		
	}

	
	// Update is called once per frame
	void Update () {
		
	}

	protected void resolve() {
		mNPC.resolveProposal (this);
	}
}
