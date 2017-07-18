using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SightEvent : Event {
	public bool onSight = true;
	public SightEvent() {
		eventType = "sight";
	}
}
