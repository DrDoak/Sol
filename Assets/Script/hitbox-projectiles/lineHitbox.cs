using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(LineRenderer))]
public class lineHitbox : hitbox {
	public float range = 1000;
	private LineRenderer line;
	public Vector3 aimPoint = new Vector3();
	bool foundPoint = false;
	Vector2 endPoint = new Vector2();

	void Start ()
	{
		line = GetComponent<LineRenderer> ();
		line.SetVertexCount (2);
		line.startWidth = 0.2f;
		line.startColor = Color.red;
	}

	void Update ()
	{
		base.updateTick ();
		RaycastHit2D hit;
		hit = Physics2D.Raycast (transform.position, aimPoint, range); // transform.position + (transform.right * (float)offset) can be used for casting not from center.
		if (foundPoint) {
			line.SetPosition (0, transform.position);
			line.SetPosition (1, endPoint);
		} else if (hit) {
			line.SetPosition (0, transform.position);
			line.SetPosition (1, hit.point);
			Collider2D collider = hit.collider;
			string hitType = OnTriggerEnter2D (collider);
			foundPoint = true;
			if (hitType == "reflect") {
				getReflected (hit.point);
			}
			endPoint = new Vector3 (hit.point.x, hit.point.y, 0f);
		} else {
			line.SetPosition (0, transform.position);
			line.SetPosition (1, transform.position + new Vector3((aimPoint * range).x,(aimPoint * range).y,0f));
		}
	}

	public void getReflected(Vector2 hitPoint) {
		string fac = "player";
		float offsetX = Random.Range (-15, 15) / 100f;
		float offsetY = Random.Range (-15, 15) / 100f;
		Vector2 realD = new Vector2 (-aimPoint.x + offsetX, -aimPoint.y + offsetY);
		Vector2 realKB = new Vector2 (-knockback.x, knockback.y);
		realD = Vector2.ClampMagnitude (realD, 1.0f);

		Vector3 newPos = new Vector3(hitPoint.x - aimPoint.x, hitPoint.y - aimPoint.y, 0);
		GameObject go = Instantiate(gameObject,newPos,Quaternion.identity) as GameObject; 
		lineHitbox line = go.GetComponent<lineHitbox> ();
		line.setRange (range);
		line.setDamage (damage);
		line.setAimPoint (realD);
		line.setHitboxDuration( hitboxDuration);
		line.setKnockback (realKB);
		line.setFixedKnockback (true);
		line.setFaction (fac);
		line.creator = gameObject;
		line.stun = stun;
		line.mAttr = mAttr;
	}

	public void setRange(float r) {
		range = r;
	}
	public void setAimPoint(Vector2 aP) {
		aimPoint = aP;
	}
}