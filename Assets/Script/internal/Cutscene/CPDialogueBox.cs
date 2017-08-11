using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPDialogueBox : CutscenePiece {
	[Multiline]
	public string text;
	public string cutsceneAnim = "none";
	public string talkTo = "none";
	Character speaker;
	Character talkTarget;
	bool type = true;
	TextboxManager tm;
	DialogueSequence ds;
	List<DialogueSequence> allDS;
	void Start() {
		tm = FindObjectOfType<TextboxManager> ();
		allDS = new List<DialogueSequence> ();
		init ();
	}
	public override void onEventStart() {
		//tb = tm.addTextbox (text, cm.findChar (targetCharName).gameObject, type);
		talkTarget = cm.findChar (talkTo);
		speaker = cm.findChar (targetCharName);
		ds = new DialogueSequence ();
		allDS.Add (ds);
		string lastText = "";
		string lastAnim = cutsceneAnim;
		int i = 0;
		bool specialGroup = false;
		while (i < text.Length) {
			char lastC = text.ToCharArray () [i];
			if (lastC == '`') {
				specialGroup = true;
				lastText += lastC;
			} else if (!specialGroup && lastC == ':' && lastText.Length < 18) {
				cm.setDialogueSequence(targetCharName,ds);
				ds = new DialogueSequence ();
				allDS.Add (ds);
				targetCharName = lastText;
				lastText = "";
			} else if (lastC == '\n' || lastC == '|') {
				if (lastAnim == "none") {
					ds.addTextbox (lastText);
				} else {
					ds.addTextbox (lastText,cutsceneAnim);
				}
				lastText = "";
				specialGroup = false;
			} else {
				lastText += lastC;
			}
			i += 1;
		}
		if (lastAnim == "none") {
			ds.addTextbox (lastText);
		} else {
			ds.addTextbox (lastText,cutsceneAnim);
		}
		cm.setDialogueSequence(targetCharName,ds);
		allDS[0].startSequence ();
		ds = allDS [0];
		allDS.Remove (ds);
	}
	public override void activeTick (float dt) {
		if (talkTarget != null) {
			if (talkTarget.transform.position.x > speaker.transform.position.x) {
				speaker.GetComponent<Movement> ().setFacingLeft (false);
			} else {
				speaker.GetComponent<Movement> ().setFacingLeft (true);
			}
		}
		if (ds.finished) {
			if (allDS.Count > 0) {
				ds = allDS [0];
				ds.startSequence ();
				allDS.Remove (ds);
			} else {
				parent.progressEvent ();
			}
		}
	}
	public override void onComplete() {
		ds.endDialogue ();
	}
}
