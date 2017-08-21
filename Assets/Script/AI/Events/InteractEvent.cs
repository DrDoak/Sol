using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractEvent : Event {
	public Interactable targetedObj;
	public bool isCharInteraction = false;
	public Character listenerChar;
	public InteractEvent() {
		eventType = "interact";
	}
}
