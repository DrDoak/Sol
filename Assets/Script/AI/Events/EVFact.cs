﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EVFact : Event {
	public Assertion assertion;
	public bool isDuplicate = false;
	public bool isAsk = false;
	public EVFact() {
		eventType = "fact";
	}
}
