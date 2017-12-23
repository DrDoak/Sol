using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EVHitConfirm : EVAttack {
	public GameObject ObjectHit;
	public EVHitConfirm() {
		eventType = "hit";
	}
}
