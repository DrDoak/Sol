using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlEtiquette : Goal {
	KNManager km;
	public GlEtiquette() {
		km = GameObject.FindObjectOfType<KNManager> ();
		registerEvent ("interact", interactEvent,startDialogue);
		registerEvent ("fact", factEvent,answerQuestion);
	}

	public float interactEvent(Event e,Relationship ci,Personality p) {
		//Debug.Log ("Interact event!!!! from GLEtiquette");
		//Debug.Log ("OpenAllegience: " + p.opennessAllegiance + " Favor rating: " + favor);
		if (!ci.openHostile) {
			float favor = (ci.favorability * ci.relevance);
			favor *= (p.opennessAllegiance * 2.0f);
			favor += p.agreeableness + 0.05f;
			return favor;
		}
		return 0f;
	}
	void startDialogue(Proposal p) {
		DialogueUnit du = new DialogueUnit ();
		du.speaker = p.mEvent.targetChar;
		du.listener = mChar;
		du.addDialogueOptions (getDialogueOptions (p.mEvent.targetChar));
		p.mEvent.targetChar.processDialogueRequest (mChar,du);
		du.startSequence ();
	}
	public float factEvent(Event e, Relationship ci, Personality p) {
		Assertion a = e.assertion;
		//Debug.Log("evaluating fact event: " + a.Source.Owner);
		if (a.Source.Owner != null)
			return 1.0f;
		return 0f;
	}

	void answerQuestion(Proposal p) {
		Assertion a = p.mEvent.assertion;
		Assertion statement = new Assertion ();
		statement.AddSubject (km.FindOrCreateSubject (p.mNPC.name));
		statement.AddVerb (km.FindOrCreateVerb ("know"),true);
		statement.AddReceivor (a);
		mChar.speaker.EmitResponse(mChar.speaker.Convey (statement,a.Source.Owner));
	}

	public virtual List<DialogueOption> getDialogueOptions(Character otherChar) {
		List<DialogueOption> options = new List<DialogueOption> (3);
		/*DialogueOption gossip = new DialogueOption ();
		gossip.text = "Gossip";
		gossip.responseFunction = startGossip;
		options.Add (gossip);*/

		DialogueOption askAbout = new DialogueOption ();
		askAbout.text = "Ask About...";
		askAbout.responseFunction = startAsk;
		options.Add (askAbout);

		DialogueOption talkAbout = new DialogueOption ();
		talkAbout.text = "Inform...";
		talkAbout.responseFunction = startTalk;
		options.Add (talkAbout);

		DialogueOption leave = new DialogueOption ();
		leave.text = "Leave";
		leave.responseFunction = leave.closeSequence;
		options.Add (leave);
		return options;
	}

	void startGossip(DialogueOption d) {
		d.closeSequence ();
		km.CreateSubjectList(d.speaker,d.listener,true);
	}

	void startAsk(DialogueOption d) {
		d.closeSequence ();
		km.CreateSubjectList(d.speaker,d.listener,true);
	}

	void startTalk(DialogueOption d) {
		d.closeSequence ();
		km.CreateSubjectList(d.speaker,d.listener,false);
	}
}
