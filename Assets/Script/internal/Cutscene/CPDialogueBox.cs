﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPDialogueBox : CutscenePiece {
	public string text;
	public string animation = "none";
	public string talkTo = "none";
	Character speaker;
	Character talkTarget;
	bool type = true;
	TextboxManager tm;
	textbox tb;
	void Start() {
		tm = FindObjectOfType<TextboxManager> ();
		init ();
	}
	public override void onEventStart() {
		tb = tm.addTextbox (text, cm.findChar (targetCharName).gameObject, type);
		talkTarget = cm.findChar (talkTo);
		speaker = cm.findChar (targetCharName);
	}
	public override void activeTick (float dt) {
		if (talkTarget != null) {
			if (talkTarget.transform.position.x > speaker.transform.position.x) {
				speaker.GetComponent<Movement> ().setFacingLeft (false);
			} else {
				speaker.GetComponent<Movement> ().setFacingLeft (true);
			}
		}
		if (tb == null || tb.conclude) {
			parent.progressEvent ();
		}
	}
}
