using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.EventSystems;

public class StatusTab : MonoBehaviour{ //, IPointerClickHandler {
	// Update is called once per frame
	StatusMenuManager smm;
	GameObject panelPrefab;
	void Start () {
		smm = FindObjectOfType<StatusMenuManager> ();
		panelPrefab = gameObject.transform.Find ("panel").gameObject;
	}
	void Update () {}
	//-----------------------------
	public void OnPointerClick()
	{
		smm.currentTab.close ();
		Debug.Log ("on pointer click");
		panelPrefab.SetActive (true);
		smm.currentTab = this;
	}

	public void close() 
	{
		
		panelPrefab.SetActive (false);
	}
}