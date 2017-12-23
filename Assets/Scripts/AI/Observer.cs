using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observer : MonoBehaviour {

	Character c;
	Movement m;
	public float detectionRange = 15.0f;

	List<Observable> visibleObjs = new List<Observable>();
	float sinceLastScan;
	float scanInterval = 0.5f;
	float postLineVisibleTime = 3.0f;

	// Use this for initialization
	void Start () {
		m = GetComponent<Movement> ();
		c = GetComponent<Character> ();
		sinceLastScan = UnityEngine.Random.Range (0.0f, scanInterval);
	}

	void Update() {
		if (sinceLastScan > scanInterval) {
			scanForEnemies ();
		}
		sinceLastScan += Time.deltaTime;
	}

	void scanForEnemies() {
		//Debug.Log (gameObject + " is scanning, found " + allObs.Length);
		Observable[] allObs = FindObjectsOfType<Observable> ();
		float lts = Time.realtimeSinceStartup;
		foreach (Observable o in allObs) {
			Vector3 otherPos = o.transform.position;
			Vector3 myPos = transform.position;
			if (o.gameObject != gameObject && otherPos.x < myPos.x && m.facingLeft ||
			    otherPos.x > myPos.x && !m.facingLeft) {
				float cDist = Vector3.Distance (otherPos, myPos);
				if (cDist < detectionRange) {
					RaycastHit2D[] hits = Physics2D.RaycastAll (myPos, otherPos - myPos, cDist);
					Debug.DrawRay (myPos, otherPos - myPos, Color.green);
					float minDist = float.MaxValue;
					foreach (RaycastHit2D h in hits) {
						GameObject oObj = h.collider.gameObject;
						if (oObj != gameObject ) {
							minDist = Mathf.Min(minDist,Vector3.Distance (transform.position,h.point));
						}
					}
					float diff = Mathf.Abs (cDist - minDist);
					if (diff < 1.0f) {
						if (o.c != null) {
							if (!c.charInfo.ContainsKey (o.c)) {
								Relationship cin = new Relationship ();
								cin.parentChar = c;
								cin.lastTimeSeen = lts;
								c.charInfo.Add (o.c, cin);
							} else {
								c.charInfo [o.c].lastTimeSeen = lts;
								c.charInfo [o.c].canSee = true;
							}
						}
						if (!visibleObjs.Contains (o)) {
							onSight (o);
							o.addObserver (this);
							visibleObjs.Add (o);
						}
					}
				}
			}
		}
		if (visibleObjs.Count > 0) {
			for (int i= visibleObjs.Count - 1; i >= 0; i --) {
				Observable o = visibleObjs [i];
				if (o == null || c.gameObject == null) {
					visibleObjs.RemoveAt (i);
				} else if (o.c && lts - c.charInfo [o.c].lastTimeSeen > postLineVisibleTime) {
					o.removeObserver (this);
					outOfSight (o, true);
					visibleObjs.RemoveAt (i);
				} else if (o.c && Mathf.Abs(lts - c.charInfo [o.c].lastTimeSeen) > 0.05f 
					&& c.charInfo [o.c].canSee == true){
					c.charInfo [o.c].canSee = false;
					outOfSight (o, false);
				}
			}
		}
		sinceLastScan = 0f;
	}
	public virtual void onSight(Observable o) {
		if (c != null) {
			EVSight se = new EVSight ();
			se.observable = o;
			se.targetChar = o.c;
			c.respondToEvent (se);
		}
	}
	public virtual void outOfSight(Observable o,bool full) {
		if (full) {
		} else {
			if (c != null) {
				EVSight se = new EVSight ();
				se.targetChar = o.c;
				se.observable = o;
				se.onSight = false;
				c.respondToEvent (se);
			}
		}
	}
	public void respondToEvent(Event e) {
		//Debug.Log ("...");
		if (c != null) {
			//Debug.Log ("C responding to Event: " + e.eventType);
			c.respondToEvent (e);
		}
	}
	void OnDestroy() {
		foreach (Observable o in visibleObjs) {
			o.removeObserver (this);	
		}
	}
}

//Enemy detection
/*void scanForEnemies() {
		Character[] allChars = FindObjectsOfType<Character> ();
		float lts = Time.realtimeSinceStartup;
		foreach (Character c in allChars) {
			if (c != this  && c.transform.position.x < transform.position.x && c.movt.facingLeft || 
				c.transform.position.x > transform.position.x && !c.movt.facingLeft) {
				float cDist = Vector3.Distance (c.transform.position, transform.position);
				if (cDist < detectionRange) {
					RaycastHit2D [] hits = Physics2D.RaycastAll(transform.position,c.transform.position - transform.position,cDist);
					Debug.DrawRay(transform.position,c.transform.position - transform.position,Color.green);
					float minDist = float.MaxValue;
					foreach (RaycastHit2D h in hits) {
						GameObject o = h.collider.gameObject;
						if (o != gameObject ) {
							minDist = Mathf.Min(minDist,Vector3.Distance (transform.position,h.point));
						}
					}
					float diff = Mathf.Abs (cDist - minDist);
					if ( diff < 1.0f) {
						if (!c.charInfo.ContainsKey (c)) {
							Relationship cin = new Relationship ();
							cin.parentChar = this;
							cin.lastTimeSeen = lts;
							c.charInfo.Add (c, cin);
						} else {
							c.charInfo [c].lastTimeSeen = lts;
							c.charInfo [c].canSee = true;
						}
						if (!visibleCharacters.Contains (c)) {
							onSight (c);
							c.ob.addObserver (this);
							visibleCharacters.Add (c);
						}
					}
				}
			}
		}
		if (visibleCharacters.Count > 0) {
			for (int i= visibleCharacters.Count - 1; i >= 0; i --) {
				Character c = visibleCharacters [i];
				if (c == null || c.gameObject == null) {
					visibleCharacters.RemoveAt (i);
				} else if (lts - charInfo [c].lastTimeSeen > postLineVisibleTime) {
					c.removeObserver (this);
					outOfSight (c, true);
					visibleCharacters.RemoveAt (i);
				} else if (Math.Abs(lts - charInfo [c].lastTimeSeen) > 0.05f 
					&& charInfo [c].canSee == true){
					charInfo [c].canSee = false;
					outOfSight (c, false);
				}
			}
		}

		sinceLastScan = 0f;
	}*/