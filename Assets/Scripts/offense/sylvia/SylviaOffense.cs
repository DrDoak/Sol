using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SylviaOffense : MonoBehaviour {

	public GameObject KnifePrefab;
	public int numKnives; 
	List<SyKnife> knives;
	void Start () {	
		knives = new List<SyKnife> ();
		Vector3 pos = transform.position;
		for (int i = 0; i < numKnives; i++) {
			GameObject go = Instantiate (KnifePrefab, new Vector3 (pos.x + Random.Range (-1f, 1f), pos.y + Random.Range (-1f, 1f), pos.z), Quaternion.identity);
			go.GetComponent<SyKnife> ().User = this;
			knives.Add (go.GetComponent<SyKnife> ());
		}
	}
	void Update () { }

	/*public void TargetPoint(Vector2 target) {
		foreach (SyKnife k in knives) {
			if (k.Available) {
				k.TargetPoint (target);
				break;
			}
		}
	}

	public void TargetPoint(Vector2 target, int numberKnives) {
	}*/

	public void TargetPoint(List<Vector2> targets,float delay) {
		int next = 0;
		float curr_delay = delay;
		//List<SyKnife> newTargets = new List<SyKnife> ();
		foreach (SyKnife k in knives) {
			
			if (next >= targets.Count) {
				break;
			} else if (k.Available) {
				k.TargetPoint (targets[next],curr_delay);
				curr_delay += delay;
				next += 1;
			}
		}
		knives.Reverse();
	}
}
