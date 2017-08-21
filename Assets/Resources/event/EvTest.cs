using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvTest : WorldEvent {
	void Start() {
		Debug.Log ("Event starting");
		DialogueUnit ds = new DialogueUnit ();
		ds.addTextbox ("Sample textbox 1");
		ds.addTextbox ("Another textbox 2");
		FindObjectOfType<CharacterManager> ().setDialogueUnit("Nacht",ds);
		Destroy (this);
	}
}
