using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SylviaOffense : MonoBehaviour {

	public GameObject KnifePrefab;
	public int NumKnives; 

	List<SyKnife> m_knives;
	Fighter m_fighter;
	void Start () {	
		m_knives = new List<SyKnife> ();
		Vector3 pos = transform.position;
		m_fighter = GetComponent<Fighter> ();
		for (int i = 0; i < NumKnives; i++) {
			GameObject go = Instantiate (KnifePrefab, new Vector3 (pos.x + Random.Range (-1f, 1f), pos.y + Random.Range (-1f, 1f), pos.z), Quaternion.identity);
			go.GetComponent<SyKnife> ().User = this;
			m_knives.Add (go.GetComponent<SyKnife> ());
		}
	}
	void Update () { 
		if (m_fighter.stunTime > 0f) {
			foreach (SyKnife k in m_knives) {
				k.SetActive (false);
			}
		}
	}

	/*public void TargetPoint(Vector2 target) {
		foreach (SyKnife k in m_knives) {
			if (k.Available) {
				k.TargetPoint (target);
				break;
			}
		}
	}

	public void TargetPoint(Vector2 target, int numberm_knives) {
	}*/

	public void TargetPoint(List<Vector2> targets,float delay) {
		int next = 0;
		float curr_delay = delay;
		//List<SyKnife> newTargets = new List<SyKnife> ();
		foreach (SyKnife k in m_knives) {
			
			if (next >= targets.Count) {
				break;
			} else if (k.Available) {
				k.TargetPoint (targets[next],curr_delay);
				curr_delay += delay;
				next += 1;
			}
		}
		m_knives.Reverse();
	}
}
