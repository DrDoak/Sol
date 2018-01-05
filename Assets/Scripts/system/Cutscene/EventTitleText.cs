using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTitleText : MonoBehaviour {
	public string LevelName = "Terra Incognita";
	public string LevelDescription = "";
	// Use this for initialization
	void Update () {
		GameManager.DisplayLevelText (LevelName, LevelDescription);
		Destroy (gameObject);
	}
}
