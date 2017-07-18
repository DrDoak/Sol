using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour {

	public GameObject gameManager;
	public GameObject guiHandler;

	// Use this for initialization
	void Awake () {
		if (GameManager.manager == null)
			Instantiate (gameManager);
		if (GUIHandler.instance == null)
			Instantiate (guiHandler);
	}

}
