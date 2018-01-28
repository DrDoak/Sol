using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndZone : MonoBehaviour {

	private GameManager gameManager;

	internal void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.GetComponent<Playable>()) {
			other.gameObject.GetComponent<Playable> ().transform.position = other.gameObject.GetComponent<Playable> ().startPosition;
		}
	}

	internal void Start() {
		gameManager = FindObjectOfType<GameManager> ();
	}

	internal void Update() {
	}
		
}
