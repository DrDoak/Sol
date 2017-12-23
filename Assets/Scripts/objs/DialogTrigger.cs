using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTrigger : TextboxTrigger {
	public List<string> options;
	List<DialogueOption> mOpts;

	// Use this for initialization
	void Start () {
		base.initTM ();
	}
	
	// Update is called once per frame
	void Update () {
		base.mUpdate ();
	}
	protected override void triggerText() {
		Debug.Log ("Why are you using this test function???");
		//tm.addDialogueOptions (displayText, gameObject, options);
	}
}
