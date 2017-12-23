using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour {

	public Vector3 innateSpeed;
	private Transform cameraTransform;
	public float cloudSpeed = 35.0f;
	private Vector2 lastCamera;

	// Use this for initialization
	void Start () {
		cameraTransform = FindObjectOfType<CameraFollow>().transform;
	}

	// Update is called once per frame
	void Update () {
		transform.position += innateSpeed * Time.deltaTime;
		if (innateSpeed.x < 0f && transform.position.x < (cameraTransform.position.x - 30.0f)) {
			ResetPos ();
		} else if (innateSpeed.x > 0f && transform.position.x > (cameraTransform.position.x + 30.0f)) {
			ResetPos ();
		}
		//if (cameraTransform.position.x > (cameraTransform.position.x - viewZone))
		//	ScrollRight ();
	}
	private void ResetPos() 
	{
		
		float newX = cameraTransform.position.x ;
		if (innateSpeed.x < 0f) {
			newX += Random.Range (30.0f, 60.0f);
		} else { 
			newX -= Random.Range (30.0f, 60.0f);
		}
		float newY = cameraTransform.position.y + Random.Range (-10.0f, 10.0f);
		float newZ = Random.Range (-9, 25);
		float sizeScale = (40f - (newZ + 9f)) / cloudSpeed;
		transform.localScale = new Vector3 ( sizeScale * 0.7f, sizeScale* 0.5f, sizeScale * 0.7f);
		GetComponent<ParticleSystem> ().Clear ();
		if (innateSpeed.x > 0) {
			innateSpeed = new Vector3 (sizeScale * cloudSpeed, 0.0f, 0.0f);
		} else {
			innateSpeed = new Vector3 (sizeScale * -cloudSpeed, 0.0f, 0.0f);
		}

		transform.position = new Vector3 (newX,newY,newZ);
	}
}
