using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EVInteract : Event {
	public Character Interactor;
	public Interactable Interactee;
	public bool IsCharInteraction = false;
	public Character InteracteeChar;
	public EVInteract() {
		eventType = "interact";
	}
}
