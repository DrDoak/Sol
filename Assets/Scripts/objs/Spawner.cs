using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

	public GameObject respawnObj;
	public float interval = 3.0f;
	public int max_items = 3;
	public int spawnedItems = 0;
	public bool resetTimerOnDestroy = true;
	public bool permanentObject = false;
	public string groupID = "none";
	float currentTime;
	public bool singlePointRespawn = true;
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

	void OnDrawGizmos() {
		Gizmos.color = new Color (0, 1, 0, .5f);
		Gizmos.DrawCube (transform.position, transform.localScale);
	}
	
	// Update is called once per frame
	void Update () {
		if (respawnObj) {
			
			currentTime += Time.deltaTime;
			if (currentTime > interval && spawnedItems < max_items) { 
				float newX = transform.position.x ;
				float newY = transform.position.y ;
				if (!singlePointRespawn) {
					newX += Random.Range (-transform.localScale.x / 2, transform.localScale.x / 2);
					newY += Random.Range (-transform.localScale.y / 2, transform.localScale.y / 2);
				}
				GameObject obj = GameObject.Instantiate (respawnObj, new Vector3 (newX, newY, 0), Quaternion.identity);
				spawnedItems += 1;
				//			Debug.Log (spawnedItems);
				currentTime = 0f;
				obj.AddComponent<SpawnedObj> ();
				obj.GetComponent<SpawnedObj> ().mSpawner = this;
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
