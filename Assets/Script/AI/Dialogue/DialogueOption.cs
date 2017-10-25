﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueOption {
	DialogueSequence parentSeq;
	public ListSelection parentList;
	public string text;
	public delegate void optionResponse(DialogueOption thisOption);
	public optionResponse responseFunction;
	public Character speaker;
	public Character listener;

	public DialogueOption() {}

	public void setToReturn() {
	}
	public void setDisplayText(string s) {
		text = s;
	}
	public void closeSequence(DialogueOption dop) {
		GameObject.Destroy (dop.parentList.gameObject);
	}
}
