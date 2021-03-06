﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ListSelection : MonoBehaviour{
	public GameObject kEntry;
	public DialogueUnit masterSequence;
	public bool Escapable = true;
	public bool AlphaSort = true;
	List<DialogueOption> prevEntries;
	List<DialogueOption> fullEntries;
	List<string> displayedNames;
	List<GameObject> entries;
	int currEntry = 0;
	InputField inputField;
	TextMeshProUGUI m_titleText;
	int lastChar = 0;
	public delegate void optionResponse(DialogueOption thisOption);
	void Awake() {
		displayedNames = new List<string> ();
		fullEntries = new List<DialogueOption> ();
		entries = new List<GameObject> ();
		inputField = transform.Find ("SearchField").GetComponent<InputField> ();
		m_titleText = transform.Find ("TitlePrompt").GetComponentInChildren<TextMeshProUGUI> ();
		inputField.Select ();
	}
	public void SetTitle(string s) {
		m_titleText.text = s;
	}
	void Update() {
		if (inputField.text.Length != lastChar) {
			searchList (inputField.text);
		}
		if (Escapable && Input.GetButtonDown ("Cancel")) {
			masterSequence.closeSequence ();
			//Debug.Log ("Previous is: " + masterSequence.Previous);
			if (masterSequence.Previous != null) {
				masterSequence.Previous.RestartSequence ();
			}
		}
		float inputY = Input.GetAxis ("Vertical");
		if (Mathf.Abs (inputY) > 0.4f)
			changeOption (inputY);
		/*if (Input.GetButtonDown("Submit")) {
			selectHighlightedOption ();
		}*/
	}
	void changeOption(float dir) {
		if (dir > 0) {
			currEntry += 1;
		} else if (dir < 0) {
			currEntry -= 1;
		}
		if (currEntry >= entries.Count) {
			currEntry = 0;
		} else if (currEntry < 0) {
			currEntry = entries.Count - 1;
		}
		//Debug.Log ("entry: " + currEntry + " : " + displayedNames [currEntry]);
	}

	void selectHighlightedOption() {
		if (entries.Count > 0) {
			Debug.Log ("entry: " + currEntry + " : " + displayedNames [currEntry]);
			entries [currEntry].GetComponent<ListOptionButton> ().onSelect ();
		}
	}

	void searchList(string key) {
		string qKey = key.ToLower ();
		foreach (DialogueOption dOpt in fullEntries) {
			string s = dOpt.text.ToLower();
			string subs = s.Substring (0, Mathf.Min (qKey.Length, s.Length));
			if (qKey.Length == 0 || subs.Equals(qKey)) {
				addOption (dOpt);
			} else {
				removeOption (dOpt);
			}
		}
		lastChar = inputField.text.Length;
	}

	public void addOptions(List<DialogueOption> options) {
		foreach (DialogueOption o in options) {
			addOption (o);
		}
	}
	public void addOption(DialogueOption DOption) {
		string name = DOption.text;
		DOption.parentList = this;
		if (!displayedNames.Contains(name)) {
			GameObject newEntry = Instantiate (kEntry);
			newEntry.GetComponent<TextMeshProUGUI> ().text = name;
			newEntry.GetComponent<ListOptionButton> ().mDialogOption = DOption;
			newEntry.transform.SetParent(transform.Find ("List").Find ("Grid"),false);
			if (!fullEntries.Contains (DOption)) {
				fullEntries.Add (DOption);
			}
			displayedNames.Add (name);
			entries.Add (newEntry);
		}
		displayedNames.Sort ();
	}
	public void removeOption(DialogueOption DOption) {
		string name = DOption.text;
		if (displayedNames.Contains(name)) {
			List<GameObject> preserveList = new List<GameObject>();
			foreach (GameObject e in entries) {
				if (e.GetComponent<TextMeshProUGUI> ().text == name) {
					displayedNames.Remove (name);
					Destroy(e);
				} else {
					preserveList.Add(e);
				}
			}
			entries = preserveList;
		}
		changeOption (0f);
		displayedNames.Sort ();
	}

	public List<DialogueOption> GetOptions() {
		return fullEntries;
	}
}