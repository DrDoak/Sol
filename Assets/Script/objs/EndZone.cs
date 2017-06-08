using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndZone : MonoBehaviour {

	private GameManager gameManager;
//	Controller2D controller;
//	public Text WinMessage;
//	private bool display =  false;
//	private float displayTime = 3f;
//	private float displayStart;
//	private float displayTimePassed = 0f;

	internal void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.GetComponent<Player>()) {
//			WinMessage.text = "Player 1 wins!";
//			display = true;
//			displayStart = Time.time;
//			displayTimePassed = 0f;
//			controller.gameOver = true;
//			controller.winner = 1;
			gameManager.winner = 1;
			gameManager.gameOver = true;
			other.gameObject.GetComponent<Player> ().transform.position = other.gameObject.GetComponent<Player> ().startPosition;
		}
	}

	internal void Start() {
		gameManager = FindObjectOfType<GameManager> ();
//		WinMessage.text = "";
//		controller = FindObjectOfType<Controller2D> ();
	}

	internal void Update() {
//		Debug.Log ("start: " + displayStart + ", passed: " + displayTimePassed + ", display=" + display);
//		if (display) {
//			if (displayTimePassed < displayTime) {
//				displayTimePassed = Time.time - displayStart;
//			} else {
//				display = false;
//				WinMessage.text = "";
//			}
//		}
	}

//	void OnGUI() {
//		GUI.Label (new Rect (0, 0, Screen.width, Screen.height), WinMessage.text);
//	}
}
