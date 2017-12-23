using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPDialogueBox : CutscenePiece {
	[Multiline]
	public string text;
	public string talkTo = "none";
	Character speaker;
	DialogueParser speakerParser;

	void Start() {
		init ();
	}
	public override void onEventStart() {
		speaker = cm.findChar (targetCharName);
		speakerParser = speaker.GetComponent<DialogueParser> ();
		Debug.Log (speakerParser);
		speaker.say (text, talkTo);
	}
	public override void activeTick (float dt) {
		if (!speakerParser.isSpeaking) { 
			parent.progressEvent();
		}
	}
	public override void onComplete() {
		Debug.Log ("ending dialogue");
		speaker.endDialogue ();
	}
}
