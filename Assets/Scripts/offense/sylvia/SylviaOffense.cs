using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SylviaOffense : Fighter {

	public GameObject KnifePrefab;
	public int NumKnives; 

	List<SyKnife> m_knives;
	Fighter m_fighter;
	private float m_createTime = 0f;
	void Start () {	
		init ();
		m_knives = new List<SyKnife> ();
		m_fighter = GetComponent<Fighter> ();
		if (!WeaponSheathed)
			SetSheath (WeaponSheathed);
	}
	void Update () { 
		update ();
		if (m_fighter.stunTime > 0f) {
			foreach (SyKnife k in m_knives) {
				k.SetActive (false);
			}
		}
		if (m_createTime > 0f) {
			m_createTime -= Time.deltaTime;
			if (m_createTime <= 0f) {
				createKnives ();
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


	public override void  SetSheath(bool ToSheath) {
		WeaponSheathed = false;
		if (ToSheath) {
			tryAttack ("sheath");
			foreach (SyKnife sk in m_knives) {
				Destroy (sk.gameObject);
			}
			m_knives.Clear ();
		} else {
			tryAttack ("unsheath");
			m_createTime = 0.5f;
		}
		WeaponSheathed = ToSheath;
	}
	void createKnives() {
		Vector3 pos = transform.position;
		for (int i = 0; i < NumKnives; i++) {
			GameObject go = Instantiate (KnifePrefab, new Vector3 (pos.x + Random.Range (-1f, 1f), pos.y + Random.Range (-1f, 1f), pos.z), Quaternion.identity);
			go.GetComponent<SyKnife> ().User = this;
			m_knives.Add (go.GetComponent<SyKnife> ());
		}
	}
}
