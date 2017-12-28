using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EVCommand : Event {
	public Character Commander;
	public KNVerb Command;
	public KNSubject Target;
	public EVCommand() {
		eventType = "command";
	}
}