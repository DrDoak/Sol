﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutscenePiece : MonoBehaviour{
	bool complete;
	[HideInInspector]
	public Cutscene parent;
	[HideInInspector]
	public GameManager gm;
	[HideInInspector]
	public CharacterManager cm;
	public int order;
	public string targetCharName = "none";
	public virtual void onEventStart() {
	}
	protected void init() {
		parent = GetComponent<Cutscene> ();
		parent.addEvent (this);
	}
	public virtual void activeTick(float dt) {
	}
	public virtual void onComplete() {
	}
}
