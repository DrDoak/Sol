using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPSpawnItem : CutscenePiece {
	public GameObject spawnObj;
	public Vector3 spawnLoc;
	Color debugColor = Color.cyan;
	Vector3 ident;
	// Use this for initialization
	void Start () {
		ident = new Vector3 (1,1,1);
		init ();
	}
	
	// Update is called once per frame
	void Update () {}
	public override void onEventStart() {
		GameObject.Instantiate (spawnObj, spawnLoc, Quaternion.identity);
		parent.progressEvent ();
	}
	void OnDrawGizmos() {
		Gizmos.color = debugColor;
		Gizmos.DrawCube (spawnLoc,new Vector3(1,1,1));
	}
}
