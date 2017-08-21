using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour {

	public float interactionPriority = 1f;
	public virtual void onInteract(Character interactor) {}

}
