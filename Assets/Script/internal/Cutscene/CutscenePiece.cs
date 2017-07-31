using System.Collections;
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
	public string targetCharName = "notSet";
	public virtual void onEventStart() {
	}
	public virtual void activeTick(float dt) {
	}
	public void completeEvent() {
		parent.progressEvent ();
	}
}
