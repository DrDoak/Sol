using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutscenePiece : MonoBehaviour{
	bool complete;
	public Cutscene parent;
	public GameManager gm;
	public CharacterManager cm;
	public int order;
	public virtual void onEventStart() {
	}
	public virtual void activeTick(float dt) {
	}
	public void completeEvent() {
		parent.progressEvent ();
	}
}
