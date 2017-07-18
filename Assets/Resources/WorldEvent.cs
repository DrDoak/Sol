using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldEvent : MonoBehaviour {

	protected CharacterManager cm;
	public string eventName = "default";
	public bool oneTime;

	void Start() {
		Debug.Log ("Start function run");
		cm = MonoBehaviour.FindObjectOfType<CharacterManager> ();
	}
}
