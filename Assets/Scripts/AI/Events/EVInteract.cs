using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EVInteract : Event {
	public Interactable targetedObj;
	public bool isCharInteraction = false;
	public Character listenerChar;
	public EVInteract() {
		eventType = "interact";
	}
}
