using System.Collections;
using System.Collections.Generic;

public class DialogueSequence  {
	public int numChars = 0;
	public string rawText = "";
	public List<DialogueUnit> allDUnits;
	public DialogueSequence parentSequence = null;
}
