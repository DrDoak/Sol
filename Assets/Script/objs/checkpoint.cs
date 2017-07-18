using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkpoint : MonoBehaviour {

	public Vector3 offset;
	// Use this for initialization
	void Start () {}
	
	// Update is called once per frame
	void Update () {}
	void OnDrawGizmos() {
		Gizmos.color = new Color (1, 1, 0, .5f);
		Gizmos.DrawCube (transform.position, transform.localScale);
	}
	internal void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.GetComponent<ReturnToCheckpoint> ()) {
			other.gameObject.GetComponent<ReturnToCheckpoint> ().setCheckpoint (transform.position + offset);
		}
	}
}
