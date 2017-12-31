using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EVAsk : Event {
	public Character Asker;
	public Character Askee;
	public EVAsk() {
		eventType = EventType.Ask;
	}
}