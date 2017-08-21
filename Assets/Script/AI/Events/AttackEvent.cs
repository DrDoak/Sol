using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEvent : Event {
	public AttackInfo attackInfo;
	public AttackEvent() {
		eventType = "attack";
	}
}
