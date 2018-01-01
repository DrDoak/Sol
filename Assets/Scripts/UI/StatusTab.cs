using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.EventSystems;

public class StatusTab : MonoBehaviour{
	// Update is called once per frame
	StatusMenuManager smm;
	GameObject panelPrefab;
	void Start () {
		smm = FindObjectOfType<StatusMenuManager> ();
		if (gameObject.transform.Find("pane")) {
			panelPrefab = gameObject.transform.Find ("pane").gameObject;
		}
		//Debug.Log ("tab start " + panelPrefab);
	}
	void Update () {}
	//-----------------------------
	public void OnPointerClick() {
		if (panelPrefab == null) {
			panelPrefab = gameObject.transform.Find ("pane").gameObject;
		} else {
			smm.currentTab.close ();
			//Debug.Log ("on pointer click");
			panelPrefab.SetActive (true);
			smm.currentTab = this;
		}
	}

	public void close() {
		panelPrefab.SetActive (false);
	}
}