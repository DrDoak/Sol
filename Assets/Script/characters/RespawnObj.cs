using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnObj : MonoBehaviour {

	public GameObject respawnObj;
	public float interval = 3.0f;
	public int max_items = 3;
	public int spawnedItems = 0;
	public bool resetTimerOnDestroy = true;
	public bool permanentObject = false;
	public string groupID = "none";
	float currentTime;
	public Vector2 focusAreaSize = new Vector2(0,0);
	// Use this for initialization
	void Start () {
		MeshRenderer mr = GetComponent<MeshRenderer> ();
		if (mr) {
			Destroy (mr);
		}
		if (interval > 30.0f) {
			currentTime = interval;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (respawnObj) {
			
			currentTime += Time.deltaTime;
			if (currentTime > interval && spawnedItems < max_items) { 
				//			Debug.Log ("spawning obj");
				//			Debug.Log (respawnObj);
				float newX = transform.position.x + Random.Range (-focusAreaSize.x / 2, focusAreaSize.x / 2);
				float newY = transform.position.y + Random.Range (-focusAreaSize.y / 2, focusAreaSize.y / 2);
				GameObject obj = GameObject.Instantiate (respawnObj, new Vector3 (newX, newY, 0), Quaternion.identity);
				spawnedItems += 1;
				//			Debug.Log (spawnedItems);
				currentTime = 0f;
				obj.AddComponent<Respawnable> ();
				obj.GetComponent<Respawnable> ().spawnPoint = this;
				if (obj.GetComponent<Attackable> ()) { 
					obj.GetComponent<Attackable> ().groupID = groupID;
				}
				if (permanentObject && obj.GetComponent<disappearing> ()) {
					Destroy (obj.GetComponent<disappearing> ());
				}
			}
		}
	}
	public void registerDestruction() {
		spawnedItems = spawnedItems - 1;
		if (resetTimerOnDestroy) {
			currentTime = 0f;
		}
	}
}
