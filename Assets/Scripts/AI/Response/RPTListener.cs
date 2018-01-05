using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPTListener : RPTemplate {
	
	public override bool match (KNSubject other)
	{
		OutputTemplate = "you";
		Debug.Log ("RPTListener Match: Listener: " + listener.name + " subject: " + other.SubjectName);
		if (listener != null) {
			return other.Equals (KNManager.GetSubject (listener.name));
		} else {
			return false;
		}
	}
}
