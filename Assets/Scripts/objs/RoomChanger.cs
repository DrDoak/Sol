using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum RoomDirection {
	LEFT,
	RIGHT,
	UP,
	DOWN,
	NEUTRAL
}

public class RoomChanger : Interactable {

	public bool oneTime = true;
	public bool onContact = true;
	public string sceneName;
	public Vector3 newPos = Vector2.zero;
	public string changerID = "none";
	public string targetID = "none";
	public RoomDirection dir;
	WorldEvent we;
	CharacterManager cm;

	void Start () {
		init();
	}
	protected void init() {
		cm = FindObjectOfType<CharacterManager> ();
	}
	void Update () {}
	void OnDrawGizmos() {
		Gizmos.color = new Color (1, 1, 0, .5f);
		Gizmos.DrawCube (transform.position, transform.localScale);
	}
	internal void OnTriggerEnter2D(Collider2D other) {
		if (onContact && other.gameObject.GetComponent<Playable> () ) {
			changeRoom (other.gameObject);
		}
	}
	protected virtual void changeRoom(GameObject go) {
		if (go.GetComponent<Playable> ()) {
			if (changerID != "none") {
				RoomDirection realDir = dir;
				string realTarget = targetID;
				if (targetID == "none") {
					realTarget = changerID;
				}
				if (realDir == RoomDirection.NEUTRAL) {
					float diffX = transform.position.x - go.transform.position.x;
					float diffY = transform.position.y - go.transform.position.y;
					if (Mathf.Abs (diffX) > Mathf.Abs (diffY)) {
						if (diffX < 0f) {
							realDir = RoomDirection.LEFT;
						} else {
							realDir = RoomDirection.RIGHT;
						}
					} else {
						if (diffY > 0f) {
							realDir = RoomDirection.UP;
						} else {
							realDir = RoomDirection.DOWN;
						}
					}
				}
				GameManager.moveItem (go, sceneName, realTarget,realDir);
			} else if (Vector2.Equals(Vector2.zero,newPos)){
				GameManager.moveItem (go, sceneName, go.gameObject.transform.position);
			} else {
				GameManager.moveItem (go, sceneName, newPos);
			}
			if (go.GetComponent<Playable> ().IsCurrentPlayer) {
				GameManager.loadRoom (sceneName);
			}
			Destroy (go);
		} else {
		}
	}
	public override void onInteract(Character interactor) {
		if (!onContact) {
			changeRoom (interactor.gameObject);
		}
	}
}