using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class NewSceneButton : MonoBehaviour, IPointerClickHandler {

	public string sceneName;
	void Start () {}
	void Update () {}

	public void OnPointerClick(PointerEventData eventData)
	{
		SceneManager.LoadScene (sceneName, LoadSceneMode.Single);
	}
}
