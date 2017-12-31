using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EventType {
	Sight,
	Attack,
	Hit,
	Interact,
	Fact,
	Command,
	Ask,
	Tell,
	Exclamation
};

public class Event {

	public EventType eventType;
	public float refreshTime = -1f;
	public Assertion assertion;
//	public float aggression;
//	public float threatLevel;
//	public float approval;

	public Event() {}
}
