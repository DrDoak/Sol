using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ListOptionButton : MonoBehaviour, IPointerClickHandler {
	public DialogueOption mDialogOption;

	void Start () {}
	void Update () {}
	public void OnPointerClick(PointerEventData eventData)
	{
		//onSelect ();
	}
	public void onSelect() {
		float index = (gameObject.transform.position.x + 15f) / 30.0f;
		int i = Mathf.RoundToInt (index);
		//FindObjectOfType<ListSelection> ().itemSelected (GetComponent<Text>().text);
		mDialogOption.responseFunction(mDialogOption);
	}
}
