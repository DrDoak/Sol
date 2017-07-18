using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomChanger : Interactable {

	public bool oneTime = true;
	public bool onContact = true;
	public string sceneName;
	public Vector3 newPos = Vector2.zero;
	public string changerID = "none";
	public string targetID = "none";
	public string dir = "neutral";
	WorldEvent we;
	CharacterManager cm;
	GameManager gm;

	void Start () {
		init();
	}
	protected void init() {
		cm = FindObjectOfType<CharacterManager> ();
		gm = FindObjectOfType<GameManager> ();
	}
	void Update () {}
	void OnDrawGizmos() {
		Gizmos.color = new Color (1, 1, 0, .5f);
		Gizmos.DrawCube (transform.position, transform.localScale);
	}
	internal void OnTriggerEnter2D(Collider2D other) {
		if (onContact && other.gameObject.GetComponent<Player> () ) {
			changeRoom (other.gameObject);
		}
	}
	protected virtual void changeRoom(GameObject go) {
		if (go.GetComponent<Player> ()) {
			if (changerID != "none") {
				string realDir = dir;
				string realTarget = targetID;
				if (targetID == "none") {
					realTarget = changerID;
				}
				if (realDir == "neutral") {
					float diffX = transform.position.x - go.transform.position.x;
					float diffY = transform.position.y - go.transform.position.y;
					if (Mathf.Abs (diffX) > Mathf.Abs (diffY)) {
						if (diffX < 0f) {
							realDir = "left";
						} else {
							realDir = "right";
						}
					} else {
						if (diffY > 0f) {
							realDir = "up";
						} else {
							realDir = "down";
						}
					}
				}
				gm.moveItem (go, sceneName, realTarget,realDir);
			} else if (Vector2.Equals(Vector2.zero,newPos)){
				gm.moveItem (go, sceneName, go.gameObject.transform.position);
			} else {
				gm.moveItem (go, sceneName, newPos);
			}
			gm.loadRoom (sceneName);
		} else {
		}
	}
	public override void onInteract(Character interactor) {
		if (!onContact) {
			changeRoom (interactor.gameObject);
		}
	}
}