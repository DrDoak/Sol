using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveClass : ScriptableObject
{
	public float testFloat;
	public string prefabName;
	public Vector3 pos;
	public float health;
	public List<string> relationsInfo;
	//public Dictionary<string,int> testDict;
	//public Dictionary<string,Dictionary<string,int>> testDouble;
	public string curDataStorage;
	public SaveClass (float w,int[] ar)
	{
		testFloat = w;
	}
}
