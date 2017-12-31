using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EVExclamation : Event {
	public Character speaker;
	public string Exclamation;
	public EVExclamation() {
		eventType = "exclamation";
	}
}
