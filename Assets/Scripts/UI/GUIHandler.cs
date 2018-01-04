using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GUIHandler : MonoBehaviour {

	public static GUIHandler instance = null;
	[TextArea(1,10)]
	public string textMessage = "";

	public Slider P1HealthBar;
	public Slider P1EnergyBar;

	private bool displayTextMessage = false;
	private float displayTime;
	private float displayStart;
	private float displayTimePassed;

	private bool flashRed = false;
	private float flashTime;
	private float flashStart;
	private float flashTimePassed;

	private bool mainMenu = false;
	private float menuTime;
	private float menuStart;
	private float menuTimePassed;

	private GameManager gameManager;

	private int attemptNumber;

	void Awake () {
		if (instance == null)
			instance = this;
		else if (instance != this) {
			Destroy (gameObject);
		}
		gameManager = FindObjectOfType<GameManager> ();

		attemptNumber = 1;
		mainMenu = false;
	}

	void Update() {
		
		var P1 = FindObjectOfType<Player> ();
		if (P1 != null) {
			var P1Controller = P1.GetComponent<Attackable> ();

			P1EnergyBar.value = P1Controller.energy;
			P1HealthBar.value = P1Controller.health;
		}			
	}

	public void displayText(string msg, float dTime) {
		displayTextMessage = true;
		textMessage = msg;
		displayTime = dTime;
		displayStart = Time.time;
		displayTimePassed = 0f;
	}
	// goes to main menu in 2 seconds
	private void GoToMainMenu(float wTime) {
		if (mainMenu == false) {
			mainMenu = true;
			menuTime = wTime;
			menuStart = Time.time;
			menuTimePassed = 0f;
		}
	}

	void OnGUI() {
		if (displayTextMessage) {
//			Debug.Log (Screen.width + ", " + Screen.height);
			var centeredStyle = GUI.skin.GetStyle("Label");
			centeredStyle.fontSize = 32;
			centeredStyle.alignment = TextAnchor.UpperCenter;
			int w = 1000;
			int h = 100;
			GUI.Label (new Rect (Screen.width/2-w/2, Screen.height/2-h/2, w, h), textMessage, centeredStyle);
		}
	}
}