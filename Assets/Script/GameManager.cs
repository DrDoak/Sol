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

	private bool gameStarted = false;

	public GameObject playerHealth;

	public float allowableError;

	void Awake () {
		winner = 0;
		gameOver = false;
		InitGame ();
		foundPlayer = false;
	}

	void InitGame() {}
		
	// Update is called once per frame
	void Update () {

		if (!gameStarted && Time.time - startTime >= introTime) {
			startGame ();
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
		} else {}
	}

	void startGame() {
		GUIHandler guihandler = FindObjectOfType<GUIHandler> ();
		if (Player1) {
			Player1.Reset ();}
		gameStarted = true;
	}
}
