using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListSelection : MonoBehaviour {
	public GameObject kEntry;
	List<DialogueOption> fullEntries;
	List<string> displayedNames;
	List<GameObject> entries;
	InputField inputField;
	int lastChar = 0;
	public delegate void optionResponse(DialogueOption thisOption);
	void Awake() {
		displayedNames = new List<string> ();
		fullEntries = new List<DialogueOption> ();
		entries = new List<GameObject> ();
		inputField = transform.Find ("SearchField").GetComponent<InputField> ();
		inputField.Select ();
	}
	void Update() {
		if (inputField.text.Length != lastChar) {
			searchList (inputField.text);
		}
	}
	void searchList(string key) {
		foreach (DialogueOption dOpt in fullEntries) {
			string s = dOpt.text;
			string subs = s.Substring (0, Mathf.Min (key.Length, s.Length));
			if (key.Length == 0 || subs.Equals(key)) {
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
			newEntry.GetComponent<Text> ().text = name;
			newEntry.GetComponent<ListOptionButton> ().mDialogOption = DOption;
			newEntry.transform.SetParent(transform.Find ("List").Find ("Grid"),false);
			if (!fullEntries.Contains (DOption)) {
				fullEntries.Add (DOption);
			}
			displayedNames.Add (name);
			entries.Add (newEntry);
		}
	}
	public void removeOption(DialogueOption DOption) {
		string name = DOption.text;
		if (displayedNames.Contains(name)) {
			List<GameObject> preserveList = new List<GameObject>();
			foreach (GameObject e in entries) {
				if (e.GetComponent<Text>().text == name) {
					displayedNames.Remove (name);
					Destroy(e);
				} else {
					preserveList.Add(e);
				}
			}
			entries = preserveList;
		}
	}
}