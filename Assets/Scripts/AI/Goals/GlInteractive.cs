using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Sight: on-sight, out of sight
 * Attack: Saw someone do an attack, may not have hit anything.
 * Hit: Hit something, could be me.
 * Interact: saw something interact with something, could be me.
 * Fact: Learned something, could be any source.
 * 
 * Command: Someone told me to do something.
 * Ask: Someone asked me something.
 * Tell: Someone told me something.
 * 
*/
public class GlInteractable : Goal {
	//KNManager km;
	public GlInteractable() {
		//km = GameObject.FindObjectOfType<KNManager> ();
		registerEvent ("interact", interactEvent,startDialogue);
		//registerEvent ("ask", factEvent,answerQuestion);
		//registerEvent ("inform", fact, answerQuestion);
	}

	public float interactEvent(Event e,Relationship ci,Personality p) {
		//Debug.Log ("OpenAllegience: " + p.opennessAllegiance + " Favor rating: " + favor);
		EVInteract evi = (EVInteract)e;
		if (evi.targetedObj == mChar.GetComponent<Interactable>() && !ci.openHostile) {
			float favor = (ci.favorability * ci.relevance);
			favor *= (p.opennessAllegiance * 2.0f);
			favor += p.agreeableness + 0.05f;
			return favor;
		}
		return 0f;
	}
	void startDialogue(Proposal p) {
		DialogueUnit du = new DialogueUnit {speaker = p.mEvent.targetChar};
		du.listener = mChar;
		du.addDialogueOptions (getDialogueOptions (p.mEvent.targetChar));
		p.mEvent.targetChar.processDialogueRequest (mChar,du);
		du.startSequence ();
	}
	public float factEvent(Event e, Relationship ci, Personality p) {
		EVFact evf = (EVFact)e;
		Assertion a = e.assertion;
		Character listener = GameObject.FindObjectOfType<CharacterManager>().findChar(a.Source.SubjectName);
		Debug.Log ("Fact event: " + a.GetID () +  " From: " + listener + " DUP: " + evf.isDuplicate);
		if (listener)
			return 1.0f;
		return 0f;
	}

	void answerQuestion(Proposal p) {
		EVFact evf = (EVFact)p.mEvent;
		Debug.Log ("Answering the question DUP: " + evf.isDuplicate);
		Assertion a = evf.assertion;
		Assertion statement = new Assertion ();
		Character me = p.mNPC;
		statement.AddSubject (KNManager.FindOrCreateSubject (me.name));
		statement.AddVerb (KNManager.FindOrCreateVerb ("know"), !evf.isDuplicate);
		statement.AddReceivor (a);
		Character listener = GameObject.FindObjectOfType<CharacterManager> ().findChar (a.Source.SubjectName);
		mChar.speaker.EmitResponse(mChar.speaker.Convey (statement, listener));
	}

	public virtual List<DialogueOption> getDialogueOptions(Character otherChar) {
		List<DialogueOption> options = new List<DialogueOption> (3);
		DialogueOption command = new DialogueOption ();
		command.text = "Command...";
		command.responseFunction = startCommand;
		options.Add (command);

		DialogueOption exclaim = new DialogueOption ();
		exclaim.text = "Exclaim...";
		exclaim.responseFunction = startExclaim;
		options.Add (exclaim);

		DialogueOption askAbout = new DialogueOption ();
		askAbout.text = "Ask About...";
		askAbout.responseFunction = startAsk;
		options.Add (askAbout);

		DialogueOption talkAbout = new DialogueOption ();
		talkAbout.text = "Tell Fact...";
		talkAbout.responseFunction = startTalk;
		options.Add (talkAbout);

		DialogueOption leave = new DialogueOption ();
		leave.text = "Leave";
		leave.responseFunction = leave.closeSequence;
		options.Add (leave);
		return options;
	}
	//Command
	void startCommand(DialogueOption d) {
		d.closeSequence ();
		KNManager.CreateExclamationList (d.speaker, d.listener,commandResponse);
	}
	void commandResponse(DialogueOption o) {
		OptionKnowledgeBase okb = (OptionKnowledgeBase)o;
		var evc = new  EVCommand();
		evc.assertion = okb.assertion;
		mChar.respondToEvent (evc);
	}

	//Exclamation
	void startExclaim(DialogueOption d) {
		d.closeSequence ();
		KNManager.CreateExclamationList (d.speaker, d.listener,exclamationResponse);
	}
	void exclamationResponse(DialogueOption o) {
		OptionKnowledgeBase okb = (OptionKnowledgeBase)o;
		var evc = new  EVExclamation();
		evc.assertion = okb.assertion;
		mChar.respondToEvent (evc);
	}

	//Ask
	void startAsk(DialogueOption d) {
		d.closeSequence ();
		KNManager.CreateSubjectList(d.speaker,d.listener,answerQuestion);
	}
	void answerQuestion(DialogueOption o) {
		OptionKnowledgeBase okb = (OptionKnowledgeBase)o;
		var evf = new EVAsk ();
		evf.assertion = okb.assertion;
		mChar.respondToEvent (evf);
	}

	void startGossip(DialogueOption d) {
		d.closeSequence ();
		KNManager.CreateSubjectList(d.speaker,d.listener);
	}

	void startTalk(DialogueOption d) {
		d.closeSequence ();
		KNManager.CreateSubjectList(d.speaker,d.listener);
	}
}
