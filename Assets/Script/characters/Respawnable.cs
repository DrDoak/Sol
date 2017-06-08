using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawnable : MonoBehaviour {

	public RespawnObj spawnPoint;
	// Use this for initialization
	void Start () {}
	
	// Update is called once per frame
	void Update () {}
	void OnDestroy () {
		spawnPoint.registerDestruction ();
	}
}
