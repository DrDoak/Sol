using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusMenuManager : MonoBehaviour {
	GameManager gm;
	bool debug = false;
	GameObject menuPrefab;
	Dictionary<string,bool> activeTabs;
	bool menuOpen = false;
	bool started = true;
	public StatusTab currentTab;
	// Use this for initialization
	void Start () {
		gm = FindObjectOfType<GameManager> ();
		menuPrefab = GameObject.Find ("MenuTabs");
		currentTab = GameObject.Find ("RelationsTab").GetComponent<StatusTab> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (started) {
			menuPrefab.SetActive (false);
			started = false;
		}
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

	public void toggleMenu() {
		Debug.Log ("toggle menu display");
		Debug.Log (menuPrefab);
		menuPrefab.SetActive (!menuOpen);
		if (currentTab != null) {
			currentTab.OnPointerClick ();
		}
		menuOpen = !menuOpen;
	}
}
