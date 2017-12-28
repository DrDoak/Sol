using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EVSight : Event {
	public bool onSight = true;
	public Observable Observee;
	public Character ObservedChar;
	public EVSight() {
		eventType = "sight";
	}
}
