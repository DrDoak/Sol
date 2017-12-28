using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EVCommand : Event {
	public Character Commander;
	public EVCommand() {
		eventType = "command";
	}
}