using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour {

	public GameObject followObj;
	public bool toFollow = false;
	public Vector3 followOffset;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (followObj != null && toFollow) {
			transform.position = new Vector3(followObj.transform.position.x + followOffset.x, followObj.transform.position.y + followOffset.y,
				followObj.transform.position.z + followOffset.z);
		}
	}
}
