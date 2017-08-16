using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusMenuManager : MonoBehaviour {
	GameManager gm;
	bool debug = false;
	GameObject menuPrefab;
	Dictionary<string,bool> activeTabs;
	// Use this for initialization
	void Start () {
		gm = FindObjectOfType<GameManager> ();

	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public void setDebug(bool debugActive) {
		Debug.Log ("Switching debug mode to: " +debug);
		debug = debugActive;
	}

	public void setTab(string key, bool active) {
		activeTabs [key] = active;
	}
	public bool getTab(string key) {
		return activeTabs [key];
	}

	public void startMenu() {
		Debug.Log ("starting menu display");
	}
}
