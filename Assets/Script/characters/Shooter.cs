using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (HitboxMaker))]
public class Shooter : MonoBehaviour {

	public GameObject defaultProjectile;

	public AudioClip ShotSound;
	// Use this for initialization
	void Start () {}
	// Update is called once per frame
	void Update () {}

	public Projectile fire(Vector2 offset,bool facingLeft,string faction) {
		return fire (offset, facingLeft, faction, defaultProjectile);
	}
	public Projectile fire(Vector2 offset,bool facingLeft,string faction,GameObject shotPrefab) {
		Vector3 newPos = new Vector3(transform.position.x + offset.x, transform.position.y + offset.y, 0);
		GameObject go = Instantiate(shotPrefab,newPos,Quaternion.identity) as GameObject; 
		Projectile proj = go.GetComponent<Projectile> ();
		if (ShotSound != null) {AudioSource.PlayClipAtPoint (ShotSound, gameObject.transform.position);}
		hitbox newBox = go.GetComponentInChildren<hitbox> ();

		newBox.creator = gameObject;
		newBox.setFaction (faction);
		if (facingLeft) {
			proj.projectileSpeed = new Vector3 (-proj.projectileSpeed.x, proj.projectileSpeed.y, 0f);
		}
		proj.facingLeft = facingLeft;
		return proj;
	}

	public void registerHit(GameObject otherObj) {
	}
}
