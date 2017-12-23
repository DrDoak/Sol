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
	//public Slider P2EnergyBar;
	//public Slider P2EnergyShower;
	//private string P2EnergyShowing;
	//public Image P2EnergyBarFill;

	//public GameObject P1Instructions;
	//public GameObject P2Instructions;

	//public Color leftColor;
	//public Color rightColor;

	//public Dictionary<string, Button> allButtons;
	//public Dictionary<string, Spawnable> allPowers;

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
		/*allButtons = gameManager.allButtons;
		allPowers = gameManager.allPowers;
		P2EnergyShower.gameObject.SetActive (false);
		P2EnergyShowing = "";

		P2Instructions.gameObject.SetActive (true);*/
		//P1Instructions.gameObject.SetActive (true);
		attemptNumber = 1;
		mainMenu = false;
	}

	void Update() {
		/*
		var P1 = FindObjectOfType<Player> ();
		var P1Controller = P1.GetComponent<Attackable> ();
		//var P2 = FindObjectOfType<PlayerCursor> ();

		//P2EnergyBar.value = P2.currentPower;

		P1EnergyBar.value = P1Controller.energy;
		P1HealthBar.value = P1Controller.health;

		if (displayTextMessage) {
			if (displayTimePassed < displayTime) {
				displayTimePassed = Time.time - displayStart;
			} else {
				displayTextMessage = false;
				textMessage = "";
			}
		}
		/*
		if (flashRed) {
			if (flashTimePassed < flashTime) {
				flashTimePassed = Time.time - flashStart;
				float fTimeRatio = flashTimePassed / flashTime;
				if (fTimeRatio <= 0.25f || (fTimeRatio > 0.5f && fTimeRatio <= 0.75f)) {
					P2EnergyBarFill.color = Color.red;
				} else {
					P2EnergyBarFill.color = Color.yellow;
				}
			} else {
				flashRed = false;
			}
		}*/

		/*
		if (mainMenu) {
			if (menuTimePassed < menuTime) {
				menuTimePassed = Time.time - menuStart;
			} else {
				SceneManager.LoadScene ("MainMenu", LoadSceneMode.Single);
			}
		}
		*/
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