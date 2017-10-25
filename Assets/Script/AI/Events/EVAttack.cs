using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EVAttack : Event {
	public AttackInfo attackInfo;
	public EVAttack() {
		eventType = "attack";
	}
}
