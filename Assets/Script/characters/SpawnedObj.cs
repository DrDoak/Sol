using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnedObj : MonoBehaviour {
	public Spawner mSpawner;

	void OnDestroy() {
		mSpawner.registerDestruction ();
	}
}
