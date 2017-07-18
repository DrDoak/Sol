using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class SaveObjManager : Editor {
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
		Debug.Log ("________________ON LOAD ROOM!: " + roomName);
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
		Debug.Log ("Recreating items for room: " + RoomName);
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
		DelCharData (item);
		CharacterSaveContainer cc = LoadChars(savePath + newRoom);
		item.pos = newPos;
		string json = JsonUtility.ToJson(new CharacterSaveContainer());
		//Debug.Log (json);
		cc.actors.Add (item);
		//Debug.Log ("items: " + item);
		//Debug.Log ("cc: " + cc);
		//Debug.Log ("actors: " + cc.actors);
		Save (savePath + newRoom, cc);
	}
	public void moveItem(CharData item,string newRoom,string newID, string dir) {
		Debug.Log ("moving item: " + item.name + " to " + newRoom);
		DelCharData (item);
		CharacterSaveContainer cc = LoadChars(savePath + newRoom);
		item.targetID = newID;
		item.targetDir = dir;
		cc.actors.Add (item);

		Debug.Log ("items: " + item);
		Debug.Log ("cc: " + cc);
		Debug.Log ("actors: " + cc.actors);
		Save (savePath+newRoom, cc);
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
		Debug.Log ("items to recreate: " + charContainer.actors.Count);
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
		//Debug.Log ("loading Chars from path: " + path);
		if (File.Exists(path+ ".txt"))
		{
			Debug.Log("char data found");
			string json = File.ReadAllText(path+ ".txt");
			Debug.Log ("found text; " + json);
			return JsonUtility.FromJson<CharacterSaveContainer>(json);
		} else {
			Debug.Log("no save data found, creating new file");
			charContainer = new CharacterSaveContainer();
			SaveActors(path,charContainer);
			return charContainer;
		} 

	}
	public static Character CreateChar(string path, Vector3 position, Quaternion rotation) {
		Debug.Log ("instantiating new object: " + path);
		GameObject prefab = Resources.Load<GameObject>(path);
		GameObject go = GameObject.Instantiate(prefab, position, rotation) as GameObject;
		Character actor = go.GetComponent<Character>() ?? go.AddComponent<Character>();
		actor.recreated = true;
		return actor;
	}
	public static Character CreateChar(CharData data, string path, Vector3 position, Quaternion rotation) {
		Character actor = null;
		if (data.targetID != null) {
			Vector3 nv = data.pos;
			bool found = false;
			foreach (RoomChanger rm in roomChangers) {
				if (rm.changerID == data.targetID) {
					if (data.targetDir == "left") {
							nv = rm.transform.position - new Vector3 (rm.GetComponent<BoxCollider2D> ().size.x + 3f, 0f);
					} else if (data.targetDir == "right") {
							nv = rm.transform.position + new Vector3 (rm.GetComponent<BoxCollider2D> ().size.x + 3f, 0f);
					} else if (data.targetDir == "up") {
							nv = rm.transform.position + new Vector3 (0f, rm.GetComponent<BoxCollider2D> ().size.y + 3f, 0f);
					} else if (data.targetDir == "down") {
							nv = rm.transform.position - new Vector3 (0f, rm.GetComponent<BoxCollider2D> ().size.x + 3f, 0f);
					}
					found = true;
					break;
				}
			}
			if (found) {
				Debug.Log ("recreating from room changer");
				actor = CreateChar(path, nv, rotation);
			} else {
				Debug.Log ("recreating at ... place");
				actor = CreateChar(path, nv, rotation);
			}
		} else {
			actor = CreateChar(path, data.pos, rotation);
		}
		Debug.Log ("old regID: " + data.regID);
		actor.data = data;
		return actor;
	}
	public static void AddCharData(CharData data) {
		charContainer.actors.Add(data);
	}
	public static void DelCharData(CharData data) {
		charContainer.actors.Remove (data);
	}

	//Saving --------------------
	public static void Save(string path, CharacterSaveContainer actors) {
		//OnBeforeSave();
		//ClearSave(path);
		SaveActors(path, actors);
		ClearActorList();
	}
	private static void SaveActors(string path, CharacterSaveContainer actors) {
		string json = JsonUtility.ToJson(actors);
		//Debug.Log ("jsoN: " + json);
		//Debug.Log ("save to path: " + path+".txt");
		StreamWriter sw = File.CreateText(path + ".txt");
		sw.Close();
		File.WriteAllText(path+ ".txt", json);
	}
}