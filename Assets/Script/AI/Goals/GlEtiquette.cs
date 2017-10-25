using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlEtiquette : Goal {
	Proposal answerProp;
	public GlEtiquette() {
		answerProp = new Proposal ();
		answerProp.mMethod = startDialogue;
	}

	public override void interactEvent(Event e,Relationship ci,Personality p) {
		Debug.Log ("Interact event!!!! from GLEtiquette");
		if (!ci.openHostile) {
			float favor = (ci.favorability * ci.relevance);
			favor *= (p.opennessAllegiance * 2.0f);
			favor += p.agreeableness + 0.05f;
			Debug.Log ("OpenAllegience: " + p.opennessAllegiance + " Favor rating: " + favor);
			mChar.addProposal (answerProp,e, favor);
		}
	}
	void startDialogue(Proposal p) {
		Debug.Log ("Starting a dialogue");
		DialogueUnit du = new DialogueUnit ();
		du.speaker = p.mEvent.targetChar;
		du.listener = mChar;
		du.addDialogueOptions (mChar.getDialogueOptions (p.mEvent.targetChar));
		p.mEvent.targetChar.processDialogueRequest (mChar,du);
		du.startSequence ();
	}
}
