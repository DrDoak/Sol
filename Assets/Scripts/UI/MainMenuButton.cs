using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenuButton : MonoBehaviour, IPointerClickHandler {

	public string sceneName;
	public string description;
	public bool playButton;
	TextMeshProUGUI descripBox;
	// Use this for initialization
	void Start () {
		descripBox = GameObject.FindGameObjectWithTag ("description").GetComponent<TextMeshProUGUI>();
		descripBox.text = "Select a level.";
	}
	
	// Update is called once per frame
	void Update () {
//		if (EventSystem.current.IsPointerOverGameObject ()) {
//			
//		}
	}

	public void setScene() {
		if (sceneName != "None") {
			SceneManager.LoadScene (sceneName, LoadSceneMode.Single);
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (playButton && sceneName != "") {
			setScene ();
		} else if (playButton) {
			descripBox.text = "Select a level.";
		} else {
			descripBox.text = description;
			GameObject.Find ("Play").GetComponent<MainMenuButton> ().sceneName = sceneName;
		}
	}
//	public void OnPointerOver(PointerEventData eventData)
//	{
//		Debug.Log ("Overoverover");
//	}
}
