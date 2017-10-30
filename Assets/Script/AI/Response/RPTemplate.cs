using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPTemplate {

	public Assertion a;
	public bool match(Assertion other) {
		return a.isMatch (other);
	}

	public RPPiece toPiece(Assertion a) {
		RPPiece rp = new RPPiece ();
		return rp;
	}
}
