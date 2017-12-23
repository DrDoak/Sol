using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindArea : MonoBehaviour {

	public float WindRange = 1f;
	public float MinInterval;
	public float MaxInterval;
	public Vector2 WindForce;
	public Vector2 ForceRangeX;
	public Vector2 ForceRangeY;
	List<Rigidbody2D> m_windObjs;

	float m_untilNext;

	// Use this for initialization
	void Start () {
		m_windObjs = new List<Rigidbody2D> ();
		foreach (GameObject o in GameObject.FindGameObjectsWithTag("WindObj")) {
			m_windObjs.Add (o.GetComponent<Rigidbody2D> ());
		}
		m_untilNext = Random.Range (MinInterval, MaxInterval);
	}
	
	// Update is called once per frame
	void Update () {
		m_untilNext -= Time.deltaTime;
		if (m_untilNext <= 0) {
			ExertWind ();
		}
	}
	void ExertWind() {
		Vector2 wind = new Vector2 (WindForce.x + Random.Range (ForceRangeX.x, ForceRangeX.y),
			WindForce.y + Random.Range (ForceRangeY.x, ForceRangeY.y));
		foreach (Rigidbody2D rb in m_windObjs) {
			rb.AddForce (wind);
		}
		m_untilNext = Random.Range (MinInterval, MaxInterval);
	}
	void OnDrawGizmos() {
		Gizmos.color = new Color (0f, 0.5f, 1f, .2f);
		Gizmos.DrawSphere (transform.position,WindRange);
	}
}
