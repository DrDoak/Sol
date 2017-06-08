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
		if (allButtons == null) {
			allButtons = gameManager.allButtons;
		}

		if (allPowers == null) {
			allPowers = gameManager.allPowers;
		}
*/
		/*if (Input.GetKeyDown(KeyCode.H)) {
			if (P1Instructions.activeSelf) {
				P1Instructions.gameObject.SetActive(false);
			} else {
				P1Instructions.gameObject.SetActive(true);
			}
		}*/
		/*
		if (Input.GetKeyDown(KeyCode.Mouse2)) {
			if (P2Instructions.activeSelf) {
				P2Instructions.gameObject.SetActive(false);
			} else {
				P2Instructions.gameObject.SetActive(true);
			}
		}
		*/

		var P1 = FindObjectOfType<Player> ();
		var P1Controller = P1.GetComponent<Attackable> ();
		//var P2 = FindObjectOfType<PlayerCursor> ();

		//P2EnergyBar.value = P2.currentPower;

		P1EnergyBar.value = P1Controller.energy;
		/*
		foreach(KeyValuePair<string, Button> entry in allButtons) {
			Color buttonColor;
			if (entry.Key == P2.leftObj.name) {
				buttonColor = leftColor;
			} else if (entry.Key == P2.rightObj.name) {
				buttonColor = rightColor;
			} else {
				buttonColor = Color.white;
			}

			if (P2.currentPower < allPowers[entry.Key].cost) {
				buttonColor = buttonColor - new Color(0.2f, 0.2f, 0.2f, 0f);
			}
			entry.Value.GetComponent<Image> ().color = buttonColor;

			if ((Input.mousePosition.y <= entry.Value.transform.position.y) && (Input.mousePosition.y >= entry.Value.transform.position.y - 30)
			    && (Input.mousePosition.x >= entry.Value.transform.position.x) && (Input.mousePosition.x <= entry.Value.transform.position.x + 50)) {
				P2EnergyShower.gameObject.SetActive (true);
				P2EnergyShower.value = allPowers [entry.Key].cost;
				P2EnergyShowing = entry.Key;
			} else if (entry.Key == P2EnergyShowing) {
				P2EnergyShower.gameObject.SetActive (false);
			}
		}
	*/
		if (gameManager.gameOver) {
			
			if (gameManager.winner == 1) {
				displayText ("Player 1 wins! (Attempt " + attemptNumber + ")", 3f);
				gameManager.gameOver = false;
				gameManager.winner = 0;
				P1HealthBar.value = 0;
			} else {
				attemptNumber += 1;
				displayText ("Attempt " + attemptNumber, 2f);
				gameManager.gameOver = false;
				gameManager.winner = 0;
				P1HealthBar.value = 0;
			}
		} else {
			P1HealthBar.value = P1Controller.health;
		}

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

		if (mainMenu) {
			if (menuTimePassed < menuTime) {
				menuTimePassed = Time.time - menuStart;
			} else {
				SceneManager.LoadScene ("MainMenu", LoadSceneMode.Single);
			}
		}
	}

	public void displayText(string msg, float dTime) {
		displayTextMessage = true;
		textMessage = msg;
		displayTime = dTime;
		displayStart = Time.time;
		displayTimePassed = 0f;
	//	var sound = gameManager.soundfx.gameObject.transform;
		if (gameManager.winner == 1) {
			//if (!sound.Find ("P1Win").GetComponent<AudioSource> ().isPlaying ) {
			//	sound.Find ("P1Win").GetComponent<AudioSource> ().Play ();
				GoToMainMenu (3f);
			//}
		} else {
			//sound.Find ("P1Death").GetComponent<AudioSource> ().Play ();
		}
	}
	/*
	public void P2EnergyBarFlashRed() {
		flashRed = true;
		flashTime = 0.4f;
		flashStart = Time.time;
		flashTimePassed = 0f;
	}
	*/
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