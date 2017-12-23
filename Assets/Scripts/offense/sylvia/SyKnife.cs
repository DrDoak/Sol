using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyKnife : MonoBehaviour {
	public SylviaOffense User;
	public bool Available;

	bool m_targetingPoint = false;
	float m_delay = 0.0f;
	Vector3 m_currentTarget;
	Vector2 m_speed;
	float m_targetSpeed;
	Hitbox m_hb;

	const float ACCELERATION = 0.05f;
	const float CHASE_TOLERANCE = 0.75f;
	const float PLAYER_LEASE = 1.5f;
	Vector2 MAX_KNIFE = new Vector2(20.0f,20.0f);
	SpriteRenderer m_sprite;


	ParticleSystem.MainModule part;
	TrailRenderer trail;
	Color red;
	Color blue;

	// Use this for initialization
	void Start () {
		m_sprite = GetComponent<SpriteRenderer> ();
		m_currentTarget = new Vector2 ();
		m_speed = new Vector2 ();
		m_hb = GetComponent<Hitbox> ();
		m_hb.Active = false;
		m_hb.setFaction(User.GetComponent<Attackable>().faction);

		red = new Color(1f, 0f, 0f, 1f);
		blue = new Color(0f, 0.5f, 1f, 1f);

		ParticleSystem partsys = GetComponentInChildren<ParticleSystem> ();
		part = partsys.main;
		trail = GetComponent<TrailRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (m_delay > 0f) {
			m_delay -= Time.deltaTime;
		} else if (m_targetingPoint) {
			chaseTarget ();
		} else {
			pursuePlayer ();
		}
	}

	void pursuePlayer() {
		float offset = -2f;
		if (User.GetComponent<Movement> ().facingLeft) {
			offset *= -1f;
		}
		if (Vector2.Distance (transform.position, new Vector2 (User.transform.position.x + offset, User.transform.position.y)) > PLAYER_LEASE) {
			m_speed.x += ACCELERATION * Mathf.Sign ((offset + User.transform.position.x) - transform.position.x);
			m_speed.y += ACCELERATION * Mathf.Sign (User.transform.position.y - transform.position.y);
		} else {
			Available = true;
			SetActive (false);
		}
		m_speed *= 0.92f;
		orientToSpeed (m_speed);
		transform.Translate (m_speed,Space.World);
	}

	void chaseTarget() {
		if (Vector3.Distance (transform.position, m_currentTarget) > CHASE_TOLERANCE) {
			m_speed.x += ACCELERATION * Mathf.Sign (m_currentTarget.x - transform.position.x);
			m_speed.y += ACCELERATION * Mathf.Sign (m_currentTarget.y - transform.position.y);
		} else {
			m_targetingPoint = false;
		}
		m_speed *= 0.98f;
		orientToSpeed (m_speed);
		transform.Translate (m_speed,Space.World);
	}

	void orientToSpeed(Vector2 speed) {
		//transform.RotateAround (transform.position, Vector3.forward, 1f);
		m_sprite.transform.rotation = Quaternion.Euler (new Vector3(0f,0f,Mathf.Rad2Deg * Mathf.Atan2 (speed.y, speed.x)));
	}

	public void TargetPoint(Vector3 absLocation,float delay) {
		m_currentTarget = absLocation;
		m_delay = delay;
		//m_targetSpeed = speed;
		m_targetingPoint = true;
		SetActive (true);
	}

	public void SetActive(bool active) {
		if (active) {
			Available = false;
			part.startColor = red;
			trail.startColor = red;
			trail.endColor = red;
			m_hb.Active = true;
		} else {
			m_hb.Active = false;
			m_hb.collidedObjs.Clear ();
			part.startColor = blue;
			trail.startColor = blue;
			trail.endColor = blue;
			m_targetingPoint = false;
		}

	}
}