using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KnowledgeList : MonoBehaviour {
	public GameObject kEntry;
	List<string> fullEntries;
	List<string> displayedEntries;
	List<GameObject> entries;
	InputField inputField;
	int lastChar = 0;
	void Start() {
		displayedEntries = new List<string> ();
		fullEntries = new List<string> ();
		entries = new List<GameObject> ();
		inputField = transform.Find ("SearchField").GetComponent<InputField> ();
		Debug.Log (inputField);

		addKnowledgeEntry ("bc");
		addKnowledgeEntry ("abc");
		addKnowledgeEntry ("aabc");
		addKnowledgeEntry ("aaabc");
		addKnowledgeEntry ("aaabc");

	}
	void Update() {
		if (inputField.text.Length != lastChar) {
			searchList (inputField.text);
		}
	}
	void searchList(string key) {
		foreach (string s in fullEntries) {
			string subs = s.Substring (0, Mathf.Min (key.Length, s.Length));
			if (key.Length == 0 || subs.Equals(key)) {
				addKnowledgeEntry (s);
			} else {
				removeKnowledgeEntry (s);
			}
		}
		lastChar = inputField.text.Length;
	}
	public void addKnowledgeEntries(List<string> names) {
		foreach (string s in names) {
			addKnowledgeEntry (s);
		}
	}

	public void addKnowledgeEntry(string name) {
		if (!displayedEntries.Contains(name)) {
			GameObject newEntry = Instantiate (kEntry);
			newEntry.GetComponent<Text> ().text = name;
			newEntry.transform.parent = transform.Find ("List").Find ("Grid");
			if (!fullEntries.Contains (name)) {
				fullEntries.Add (name);
			}
			displayedEntries.Add (name);
			entries.Add (newEntry);
		}
	}

	public void removeKnowledgeEntry(string name) {
		if (displayedEntries.Contains(name)) {
			List<GameObject> preserveList = new List<GameObject>();
			foreach ( GameObject e in entries) {
				if (e.GetComponent<Text>().text == name) {
					displayedEntries.Remove (name);
					Destroy(e);
				} else {
					preserveList.Add(e);
				}
			}
			entries = preserveList;
		}
	}
	public void itemSelected(string s) {
		Debug.Log ("detected this: " + s);
	}
}
