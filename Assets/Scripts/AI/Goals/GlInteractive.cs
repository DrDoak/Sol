﻿using System.Collections;
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
public class GlInteractive : Goal {
	//KNManager km;
	public GlInteractive() {
		//km = GameObject.FindObjectOfType<KNManager> ();
		registerEvent ("interact", interactEvent,startDialogue);
		//registerEvent ("ask", factEvent,answerQuestion);
		//registerEvent ("inform", fact, answerQuestion);
	}

	float interactEvent(Event e) {
		//Debug.Log ("OpenAllegience: " + p.opennessAllegiance + " Favor rating: " + favor);
		EVInteract evi = (EVInteract)e;
		//Debug.Log ("Interact Event " + evi.Interactor + " with " + evi.Interactee); 
		if (evi.Interactee == mChar.GetComponent<Interactable>() ) {
			Relationship r = mChar.getCharInfo (evi.Interactor);
			if (r.openHostile)
				return 0f;
			Personality p = mChar.PersonalityData;
			float favor = (r.favorability * r.relevance);
			favor *= (p.opennessAllegiance * 2.0f);
			favor += p.agreeableness + 0.05f;
			return favor;
		}
		return 0f;
	}
	void startDialogue(Proposal p) {
		EVInteract evi = (EVInteract)p.mEvent;
		DialogueUnit du = new DialogueUnit {speaker = evi.Interactor};
		du.listener = mChar;
		du.addDialogueOptions (getDialogueOptions (evi.Interactor));
		evi.Interactor.processDialogueRequest (mChar,du);
		du.startSequence ();
	}

	float factEvent(Event e, Relationship ci, Personality p) {
		EVFact evf = (EVFact)e;
		Assertion a = e.assertion;
		Character listener = CharacterManager.FindChar(a.Source.SubjectName);
		//Debug.Log ("Fact event: " + a.GetID () +  " From: " + listener + " DUP: " + evf.isDuplicate);
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
		statement.AddSubject (KNManager.CopySubject (me.name));
		statement.AddVerb (KNManager.CopyVerb ("know"), !evf.isDuplicate);
		statement.AddReceivor (a);
		Character listener = CharacterManager.FindChar (a.Source.SubjectName);
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
		Assertion a = new Assertion ();
		KNManager.CreateCommandList (d.speaker, a, d.listener,commandResponse);
	}
	void commandResponse(DialogueOption d) {
		d.closeSequence();
		Debug.Log ("Emitting Command Response");
		OptionKnowledgeBase okb = (OptionKnowledgeBase)d;
		var evc = new  EVCommand();
		evc.Commander = okb.speaker;
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