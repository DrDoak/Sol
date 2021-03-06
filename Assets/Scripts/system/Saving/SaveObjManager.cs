﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveObjManager {
	//Dictionary<string, Dictionary<string,GameObject>> roomItems = new Dictionary<string,Dictionary<string,GameObject>>();
	//Dictionary<string,GameObject> curRoomInfo;
	static RoomChanger [] roomChangers;
	bool second = false;
	string curRoom;
	static string savePath = "Assets/SaveData/";

	public void saveCurrentRoom() {}
	public void resetRoomData() {
		foreach (string file in Directory.GetFiles(savePath))//Directory.GetFiles(", "Delete This File.txt", SearchOption.AllDirectories)) 
		{
			File.Delete(file);      
		}
	}
	public List<string> loadRegisteredIDs() {
		List<string> ids = new List<string> ();
		foreach (string file in Directory.GetFiles(savePath))//Directory.GetFiles(", "Delete This File.txt", SearchOption.AllDirectories)) 
		{
			//File.Delete(file);      
		}
		return ids;
	}
	public void onRoomLoad(string roomName) {
		//curRoomInfo = getRoom(roomName);
		curRoom = roomName;
		roomChangers = GameObject.FindObjectsOfType<RoomChanger> ();
		recreateItems (curRoom);
		//registerPersItems (curRoom);
	}
	/*
	public void registerPersItems(string RoomName) {
		//PersItem [] ps = Object.FindObjectsOfType<PersItem>();
		//foreach (PersItem p in ps) {
			if (!curRoomInfo.ContainsKey (p.saveID)) {
				if (Vector3.Equals (p.pos, Vector3.zero)) {
					p.pos = p.gameObject.transform.position;
				}
				curRoomInfo.Add (p.saveID,p.gameObject);
			}
		//}
	}*/

	public void recreateItems(string RoomName) {
		//Debug.Log ("Recreating items for room: " + RoomName);
		LoadRoom (savePath + RoomName);
	}
	/*
	public Dictionary<string,GameObject> getRoom (string rm) {
		if (!roomItems.ContainsKey (rm)) {
			roomItems[rm] = new Dictionary<string, GameObject>();
		}
		Debug.Log (roomItems [rm]);
		return roomItems [rm];
	}
	public void addItem(PersItem pi,string roomName,GameObject go) {
		Dictionary<string,GameObject> roomDict;
		if (roomName != curRoom) {
			roomDict = getRoom (roomName);
		} else {
			roomDict = curRoomInfo;
		}
		if (!roomDict.ContainsKey (pi.saveID)) {
			if (Vector3.Equals (pi.pos, Vector3.zero)) {
				pi.pos = go.transform.position;
			}
			roomDict.Add (pi.saveID,go);
		}
	}
	public void destroyItem(PersItem pi) {
		Debug.Log (curRoomInfo);
		if (!curRoomInfo.ContainsKey (pi.saveID)) {
			curRoomInfo.Remove(pi.saveID);
			GameObject.Destroy (pi.gameObject);
		}
	}*/
	public void moveItem(CharData item,string newRoom,Vector3 newPos) {
		Debug.Log ("moving item: " + item.name + " to " + newRoom);
		refreshPersItems();
		DelCharData (item);
		CharacterSaveContainer cc = LoadChars(savePath + newRoom);
		item.pos = newPos;
		string json = JsonUtility.ToJson(new CharacterSaveContainer());
		cc.actors.Add (item);
		Save (savePath + newRoom, cc);
		ResaveRoom ();
	}
	public void moveItem(CharData item,string newRoom,string newID, RoomDirection dir) {
		refreshPersItems();
		DelCharData (item);
		CharacterSaveContainer cc = LoadChars(savePath + newRoom);
		item.targetID = newID;
		item.targetDir = dir;
		cc.actors.Add (item);
		Save (savePath+newRoom, cc);
		ResaveRoom ();
		LoadChars (savePath + curRoom);
	}
	public void refreshPersItems() {
		Character [] cList = Object.FindObjectsOfType<Character>();
		charContainer.actors.Clear ();
		foreach (Character c in cList) {
			c.StoreData ();
			//Debug.Log ("Adding character: " + c);
			charContainer.actors.Add(c.data);
		}
	}
	public void ResaveRoom() {
		//Debug.Log ("Resaved characters: " + charContainer.actors.Count);
		Save (savePath + curRoom, charContainer);
	}
	//-----------------------------------------------------------
	//-----------------------------------------------------------

	public static CharacterSaveContainer charContainer = new CharacterSaveContainer();

	public delegate void SerializeAction();
	public static event SerializeAction OnLoaded;
	public static event SerializeAction OnBeforeSave;

	//public const string playerPath = "Prefabs/Player";

	//Loading---------------
	public static void LoadRoom(string path) {
		
		charContainer = LoadChars(path);	
		//Debug.Log ("items to recreate: " + charContainer.actors.Count);
		foreach (CharData data in charContainer.actors) {
			Character c = CreateChar (data, data.prefabPath,
				data.pos, Quaternion.identity);
			c.registryCheck ();
		}
		//OnLoaded();
		//ClearActorList();
	}
	public static void ClearActorList() {
		charContainer.actors.Clear();
	}
	private static CharacterSaveContainer LoadChars(string path) {
		if (File.Exists(path+ ".txt"))
		{
			string json = File.ReadAllText(path+ ".txt");
			//Debug.Log ("Chars from path: " + path + " : " + json);
			return JsonUtility.FromJson<CharacterSaveContainer>(json);
		} else {
			//Debug.Log("no save data found, creating new file");
			CharacterSaveContainer cc = new CharacterSaveContainer();
			SaveActors(path,cc);
			return cc;
		} 

	}
	public static Character CreateChar(string path, Vector3 position, Quaternion rotation) {
		//Debug.Log ("recreating object: " + path);
		GameObject prefab = Resources.Load<GameObject>(path);
		GameObject go = GameObject.Instantiate(prefab, position, rotation) as GameObject;
		Character actor = go.GetComponent<Character>() ?? go.AddComponent<Character>();
		actor.recreated = true;
		return actor;
	}
	public static Character CreateChar(CharData data, string path, Vector3 position, Quaternion rotation) {
		//Debug.Log ("Recreating character");
		Character actor = null;
		if (data.targetID != null) {
			Vector3 nv = data.pos;
			bool found = false;
			foreach (RoomChanger rm in roomChangers) {
				if (rm.changerID == data.targetID) {
					if (data.targetDir == RoomDirection.LEFT) {
							nv = rm.transform.position - new Vector3 (rm.GetComponent<BoxCollider2D> ().size.x + 3f, 0f);
					} else if (data.targetDir == RoomDirection.RIGHT) {
							nv = rm.transform.position + new Vector3 (rm.GetComponent<BoxCollider2D> ().size.x + 3f, 0f);
					} else if (data.targetDir == RoomDirection.UP) {
							nv = rm.transform.position + new Vector3 (0f, rm.GetComponent<BoxCollider2D> ().size.y + 3f, 0f);
					} else if (data.targetDir == RoomDirection.DOWN) {
							nv = rm.transform.position - new Vector3 (0f, rm.GetComponent<BoxCollider2D> ().size.x + 3f, 0f);
					}
					found = true;
					break;
				}
			}
			if (found) {
				actor = CreateChar(path, nv, rotation);
			} else {
				actor = CreateChar(path, nv, rotation);
			}
		} else {
			actor = CreateChar(path, data.pos, rotation);
		}
		//Debug.Log ("old regID: " + data.regID);
		actor.data = data;
		return actor;
	}
	public static void AddCharData(CharData data) {
		//Debug.Log ("Adding character");
		charContainer.actors.Add(data);
	}
	public static void DelCharData(CharData data) {
		charContainer.actors.Remove (data);
		//Debug.Log (charContainer.actors.Count);
	}

	//Saving --------------------
	public static void Save(string path, CharacterSaveContainer actors) {
		//OnBeforeSave();
		//ClearSave(path);
		SaveActors(path, actors);
		//actors.actors.Clear ();
	}
	private static void SaveActors(string path, CharacterSaveContainer actors) {
		string json = JsonUtility.ToJson(actors);
		//Debug.Log ("jsoN: " + json);
		//Debug.Log ("save to path: " + path+".txt");
		//Debug.Log("Saving: " + json.ToString() + " to path: " + path);
		StreamWriter sw = File.CreateText(path + ".txt");
		sw.Close();
		File.WriteAllText(path+ ".txt", json);
	}
}