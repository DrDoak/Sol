using System.Collections;
using System.Collections.Generic;

public class DialogueOption {
	DialogueSequence parentSeq;
	public string text;
	public delegate void optionResponse(DialogueOption thisOption);
	public optionResponse responseFunction;

	public DialogueOption() {}

	public void setToReturn() {
	}
	public void setDisplayText(string s) {
	}
}
