using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observable : MonoBehaviour {

	//Character watcher:
	List<Observer> observers = new List<Observer>();
	public Movement movt;
	public Character c;
	// Use this for initialization
	void Start () {
		c = GetComponent<Character>();
		movt = GetComponent<Movement>();
	}
	
	// Update is called once per frame
	void Update () {}

	//Observers
	public void addObserver(Observer obs) {
		if (!observers.Contains (obs)) {
			observers.Add (obs);
		}
	}
	public void removeObserver(Observer obs) {
		if (observers.Contains(obs)) {
			observers.Remove (obs);
		}
	}
	public void broadcastToObservers(Event e) {
		foreach (Observer o in observers) {	
			o.respondToEvent (e);
		}
	}
}
