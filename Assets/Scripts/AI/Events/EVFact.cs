using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EVFact : Event {
	public bool isDuplicate = false;
	public EVFact() {
		eventType = EventType.Fact;
	}
}