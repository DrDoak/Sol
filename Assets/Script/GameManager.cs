using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public GameObject startObstacle;

	public static GameManager instance = null;
	public int winner = 0; // 0 for no winner, 1 for player 1, 2 for player 2
	public bool gameOver = false;
	public Player Player1;
	GameObject curPlayer;
	bool foundPlayer;

	public GameObject startmsgs;
	private float startTime;

	public GameObject audio;
	public float introTime;
//	public GameObject soundfx;

	private bool gameStarted = false;

	public GameObject playerHealth;
//	public GameObject godPower;
//	public Dictionary<string, Button> allButtons;
//	public Dictionary<string, Spawnable> allPowers;
//	public List<GodButtons> godButtons = new List<GodButtons> (); 
//	public int currIndex = 0;
//	public int maxButtons = 0;

//	public float startX; // used by PlayerCursor for deadZone

	public float allowableError;
	private BeatTracker beatTracker = null;

	void Awake () {
//		Debug.Log ("Awake");
//		if (instance == null)
//			instance = this;
//		else if (instance != this) {
//			Destroy (gameObject);
//		}
//		DontDestroyOnLoad (gameObject);
		winner = 0;
		gameOver = false;
		InitGame ();
		foundPlayer = false;
	}

	void InitGame() {
		/*
		godCursor = (GameObject)Instantiate (playerCursorPrefab);

//		Debug.Log ("init game");
		Object[] allObjs = Resources.LoadAll ("");
		allButtons = new Dictionary<string, Button> ();
		allPowers = new Dictionary<string, Spawnable> ();
//		float xPos = 0.0f;
		float xPos = Screen.width - allObjs.Length * 50.0f;
		startX = xPos;
		float maxX = 50.0f;
		foreach (Object obj in allObjs) {
			GameObject go = (GameObject)obj;

			if (go.GetComponent<Spawnable> ()) {
				godPowers.Add (go);
				Spawnable spawnInfo = go.GetComponent<Spawnable> ();
				GameObject buttonObj = (GameObject)Instantiate (prefabButton);
				Button tempButton = buttonObj.GetComponent<Button> ();

				buttonObj.transform.SetParent (GameObject.FindObjectOfType<Canvas> ().transform);
				buttonObj.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (xPos, 0.0f);
				tempButton.GetComponentsInChildren<Text> () [0].text = spawnInfo.name;
				buttonObj.GetComponent<GodButtons> ().godCursor = godCursor;
				buttonObj.GetComponent<GodButtons> ().spawnObj = go;
				maxButtons = maxButtons + 1;
				godButtons.Add (buttonObj.GetComponent<GodButtons> ());
				buttonObj.GetComponent<GodButtons> ().buttonID = maxButtons;
				if (!godCursor.GetComponent<PlayerCursor> ().initLeft) {
					godCursor.GetComponent<PlayerCursor> ().leftObj = go;
					godCursor.GetComponent<PlayerCursor> ().initLeft = true;
				} else if (!godCursor.GetComponent<PlayerCursor> ().initRight) {
					godCursor.GetComponent<PlayerCursor> ().rightObj = go;
					godCursor.GetComponent<PlayerCursor> ().initRight = true;
				}

//				GameObject textObj = (GameObject)Instantiate (prefabText);
//				Text tempText = textObj.GetComponent<Text> ();
//				textObj.transform.SetParent (GameObject.FindObjectOfType<Canvas> ().transform);
//				textObj.GetComponent<RectTransform> ().transform.position = 
//					new Vector3 (tempButton.transform.position.x+25f, tempButton.transform.position.y-38f);
//				tempText.text = spawnInfo.cost.ToString ();


				allButtons.Add (spawnInfo.name, tempButton);
				allPowers.Add (spawnInfo.name, spawnInfo);

				xPos += 50.0f;
				maxX += 50.0f;
			}
		}
		godCursor.GetComponent<PlayerCursor> ().deadX = maxX;
//		godPowerUI = (GameObject)Instantiate (godPower);
//		godPowerUI.transform.SetParent (GameObject.FindObjectOfType<Canvas> ().transform);
//		godPowerUI.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0.0f, -45.0f);

		startTime = Time.time;
		*/
	}
		
	// Update is called once per frame
	void Update () {
		if (beatTracker == null) {
			beatTracker = FindObjectOfType<BeatTracker> ();
		}

		if (!gameStarted && Time.time - startTime >= introTime) {
			startGame ();
		}

		if (!gameStarted && Time.time - startTime >= introTime - 1) {
			killAllSpawnables ();
		}

		if (! gameStarted) {
			startmsgs.transform.Find("Countdown").GetComponent<Text>().text = ( introTime - (Time.time - startTime)).ToString ();
			Player1.GetComponent<Attackable> ().health = 100;
			if (Input.GetKeyDown(KeyCode.Escape)) {
				startGame ();
			}
		} else {
			if (Input.GetKeyDown(KeyCode.Escape)) {
				SceneManager.LoadScene ("MainMenu", LoadSceneMode.Single);
			}
		}

		if (!foundPlayer) {
			curPlayer = GameObject.FindGameObjectWithTag("Player") as GameObject;
			foundPlayer = true;
			Player1 = curPlayer.GetComponent<Player> ();
//			playerHealthUI = (GameObject)Instantiate (playerHealth);
//			playerHealthUI.transform.SetParent (GameObject.FindObjectOfType<Canvas> ().transform);
//			playerHealthUI.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0.0f, -60.0f);
		} else {
//			playerHealthUI.GetComponent<Text>().text = "Player Health: " + curPlayer.GetComponent<Controller2D>().health.ToString ();
		}
//		godPowerUI.GetComponent<Text>().text = "Current Power: " + godCursor.GetComponent<PlayerCursor>().currentPower.ToString ();
		/*
		if (Input.GetMouseButton (0))
			lastMouseButtonPressed = "Left button";
		if (Input.GetMouseButton (1))
			lastMouseButtonPressed = "Right button";	
		*/
	}

	void startGame() {
		GUIHandler guihandler = FindObjectOfType<GUIHandler> ();
		//guihandler.P1Instructions.gameObject.SetActive(false);
		//guihandler.P2Instructions.gameObject.SetActive (false);

		//audio.transform.FindChild ("IntroAudio").GetComponent<AudioSource> ().Pause ();
		//audio.transform.FindChild ("GameAudio").GetComponent<AudioSource> ().Play ();
		//Destroy (startmsgs.gameObject);
		//Destroy (startObstacle.gameObject);
		killAllSpawnables ();
		if (Player1) {
			Player1.Reset ();}
		gameStarted = true;
	}

	void killAllSpawnables() {
		foreach (Spawnable enemy in FindObjectsOfType<Spawnable>()) {
			Destroy (enemy.gameObject);
		}
	}

	public bool checkOnBeat(float time = -1) {
		if (time < 0) time = Time.time;
		float t = Mathf.Abs (beatTracker.QuarterNoteActionTime - time);
		return (t < allowableError || Mathf.Abs(0.5f-t) < allowableError);
	}
}
