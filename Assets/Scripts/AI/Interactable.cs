using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour {

	public float interactionPriority = 1f;
	public virtual void onInteract(Character interactor) {}

	List<GameObject> overlappingControl = new List<GameObject> (); 

	internal void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.GetComponent<Character>() &&
			other.gameObject.GetComponent<Character>().HighlightInteractables) {
			//Debug.Log ("Detected collision with interactor: " + gameObject);
			if (overlappingControl.Count == 0) {
				Color newCol = GetComponent<SpriteRenderer> ().color;
				newCol.b -= 0.5f;
				GetComponent<SpriteRenderer>().color = newCol;
			}
			overlappingControl.Add (other.gameObject); 
		}
	} 
	internal void OnTriggerExit2D(Collider2D other) {
		if (overlappingControl.Contains(other.gameObject)) {
			//Debug.Log ("Removing: " + other.gameObject);
			overlappingControl.Remove (other.gameObject); //Removes the object from the list
			if (overlappingControl.Count == 0) {
				Color newCol = GetComponent<SpriteRenderer> ().color;
				newCol.b += 0.5f;
				GetComponent<SpriteRenderer>().color = newCol;
			}
		}
	}

}
