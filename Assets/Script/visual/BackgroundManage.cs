using UnityEngine;
using System.Collections;
//using System.Collections.Generic;

public class BackgroundManage : MonoBehaviour {

	public bool scrolling, paralax,autoScroll,lockOnCam;
	public float backgroundSize; 
	public float paralaxSpeed;
	public Vector3 innateSpeed;
	private Transform cameraTransform;
	private Transform[] layers;
	private float viewZone = 10;
	private int leftIndex;
	private int rightIndex;

	private Vector2 lastCamera;

	// Use this for initialization
	void Start () {
		cameraTransform = FindObjectOfType<CameraFollow>().transform;
		layers = new Transform[transform.childCount];
		for (int i = 0; i < transform.childCount; i++) {
			layers [i] = transform.GetChild (i);
		}
		leftIndex = 0;
		rightIndex = layers.Length - 1;
		lastCamera = new Vector2(cameraTransform.position.x,cameraTransform.position.y);
	}
	
	// Update is called once per frame
	void LateUpdate () {

		if (paralax) {
			float deltaX = cameraTransform.position.x - lastCamera.x;
			float deltaY = cameraTransform.position.y - lastCamera.y;

			transform.position += new Vector3 (deltaX * paralaxSpeed,deltaY*paralaxSpeed, 0f);
		}
		if (autoScroll) {
			transform.position += innateSpeed * Time.deltaTime;
		}
		if (lockOnCam) {
			transform.position = new Vector3 (cameraTransform.position.x, cameraTransform.position.y, transform.position.z);
		}
		lastCamera = new Vector2 (cameraTransform.position.x, cameraTransform.position.y);
		if (scrolling) {
			if (cameraTransform.position.x < (layers [leftIndex].transform.position.x + viewZone))
				ScrollLeft ();
			if (cameraTransform.position.x > (layers [rightIndex].transform.position.x - viewZone))
				ScrollRight ();
		}
	}
	private void ScrollLeft() 
	{
//		int lastRight = rightIndex;
		Vector3 oldPos = layers [leftIndex].position;
		layers[rightIndex].position = new Vector3(oldPos.x - backgroundSize,oldPos.y,oldPos.z);
		leftIndex = rightIndex;
		//Debug.Log ("Scrolling Left");
		rightIndex--;
		if (rightIndex < 0)
			rightIndex = layers.Length - 1;
	}
	private void ScrollRight()
	{
		//Debug.Log ("Scrolling Right");
//		int lastLeft = leftIndex;
		Vector3 oldPos = layers [rightIndex].position;
		layers[leftIndex].position = new Vector3(oldPos.x + backgroundSize,oldPos.y,oldPos.z);
		rightIndex = leftIndex;
		leftIndex ++;
		if (leftIndex == layers.Length)
			leftIndex = 0;
	}
}
