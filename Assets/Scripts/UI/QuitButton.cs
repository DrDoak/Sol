using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class QuitButton : MonoBehaviour, IPointerClickHandler {
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		Debug.Log ("trying to quit");
		Application.Quit ();
	}
}
