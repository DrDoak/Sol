using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EVSight : Event {
	public bool onSight = true;
	public Observable observable;
	public EVSight() {
		eventType = "sight";
	}
}
