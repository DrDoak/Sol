﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlEtiquette : Goal {
	Proposal answerProp;
	public GlEtiquette() {
		answerProp = new Proposal ();
		answerProp.mMethod = startDialogue;
	}

	public override void interactEvent(Event e,Relationship ci,Personality p) {
		if (!ci.openHostile) {
			float favor = (ci.favorability * ci.relevance);
			favor *= (p.opennessAllegiance * 2.0f);
			favor += p.agreeableness + 0.05f;
			mChar.addProposal (answerProp,e, favor);
		} 
	}
	void startDialogue(Proposal p) {
		Debug.Log ("starting dialogue");
		DialogueUnit ds = new DialogueUnit ();
		ds.speaker = p.mEvent.targetChar;
		ds.addDialogueOptions (mChar.getDialogueOptions (p.mEvent.targetChar));
		p.mEvent.targetChar.processDialogueRequest (mChar,ds);
		ds.startSequence ();
	}
}