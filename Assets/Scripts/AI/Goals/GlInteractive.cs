using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/* Events:
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
		registerEvent (EventType.Interact, interactEvent,startDialogue);
		registerEvent (EventType.Ask, questionEvent,answerQuestion);
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
		du.addDialogueOptions (getDialogueOptions (evi.Interactor), "What will you say?");
		evi.Interactor.processDialogueRequest (mChar,du);
		du.startSequence ();
	}

	float questionEvent(Event e) {
		EVAsk evf = (EVAsk)e;
		Assertion a = e.assertion;
		Character listener = CharacterManager.FindChar(a.Source.SubjectName);
		//Debug.Log ("Fact event: " + a.GetID () +  " From: " + listener + " DUP: " + evf.isDuplicate);
		if (listener)
			return 0.1f;
		return 0f;
	}

	void answerQuestion(Proposal p) {
		EVAsk evf = (EVAsk)p.mEvent;
		Assertion a = evf.assertion;
		Assertion statement = new Assertion ();
		Character me = p.mNPC;
		statement.AddSubject (KNManager.CopySubject (me.name));
		statement.AddVerb (KNManager.CopyVerb ("know"),(me.knowledgeBase.GetAssertion(a) == null));
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
		talkAbout.responseFunction = startTell;
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
		DialogueUnit du = KNManager.CreateCommandList (d.speaker, a, d.listener,commandResponse);
		du.Previous = d.GetSequence ();
		du.startSequence ();
	}
	void commandResponse(DialogueOption d) {
		d.closeSequence();
		OptionKnowledgeBase okb = (OptionKnowledgeBase)d;
		Assertion a = okb.assertion;
		var evc = new  EVCommand();
		evc.Commander = okb.speaker;
		evc.Command = a.Verb;
		evc.Target = a.Receivors [0];
		evc.assertion = a;
		mChar.respondToEvent (evc);
	}

	//Exclamation
	void startExclaim(DialogueOption d) {
		d.closeSequence ();
		DialogueUnit du = KNManager.CreateExclamationList (d.speaker, d.listener,exclamationResponse);
		du.Previous = d.GetSequence ();
		du.startSequence ();
	}
	void exclamationResponse(DialogueOption o) {
		o.closeSequence ();
		OptionKnowledgeBase okb = (OptionKnowledgeBase)o;
		var evc = new  EVExclamation();
		evc.speaker = okb.speaker;
		evc.Exclamation = okb.assertion.Subjects [0].SubjectName;
		evc.assertion = okb.assertion;
		mChar.respondToEvent (evc);
	}

	//Ask
	void startAsk(DialogueOption d) {
		d.closeSequence ();
		DialogueUnit du = KNManager.CreateSubjectList(d.speaker,d.listener,answerQuestion);
		du.Previous = d.GetSequence ();
		du.startSequence ();
	}
	void answerQuestion(DialogueOption o) {
		OptionKnowledgeBase okb = (OptionKnowledgeBase)o;
		o.closeSequence ();
		var evf = new EVAsk ();
		evf.assertion = okb.assertion;
		Debug.Log ("Asking a question: " + evf.assertion.GetID());
		mChar.respondToEvent (evf);
	}
	void startTell(DialogueOption d) {
		d.closeSequence ();
		DialogueUnit du = KNManager.CreateSubjectList(d.speaker,d.listener,respondFact);
		du.Previous = d.GetSequence ();
		du.startSequence ();
	}
	void respondFact(DialogueOption o) {
		OptionKnowledgeBase okb = (OptionKnowledgeBase)o;
		o.closeSequence ();
		var evf = new EVFact ();
		evf.assertion = okb.assertion;
		Debug.Log ("Telling someone: " + evf.assertion.GetID() + " source: " + okb.assertion.Source);
		mChar.respondToEvent (evf);
	}
}
