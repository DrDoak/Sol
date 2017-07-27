using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPDialogueBox : CutscenePiece {
	public string text;
	public string targetCharName;
	public string animation = "none";
	public bool type = true;
	TextboxManager tm;
	textbox tb;
	void Start() {
		tm = FindObjectOfType<TextboxManager> ();
	}
	public override void onEventStart() {
		tb = tm.addTextbox (text, cm.findChar (targetCharName).gameObject, type);
	}
	public override void activeTick (float dt) {
		if (tb.gameObject == null) {
			parent.progressEvent ();
		}
	}
}
