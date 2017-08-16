using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StatusTab : MonoBehaviour, IPointerClickHandler {
	// Update is called once per frame
	void Start () {}
	void Update () {}
	//-----------------------------
	public void OnPointerClick(PointerEventData eventData)
	{
		Debug.Log ("on pointer click");
	}
}