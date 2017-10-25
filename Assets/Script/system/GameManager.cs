using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public static GameManager manager;
	public GameObject cameraPrefab;
	public float bottomOfWorld;

	GameObject curPlayer;
	CameraFollow cf;
	bool foundPlayer;
	public bool debug = false;

	new public GameObject audio;
	public float introTime;

	public SaveObjManager mSaves;
	public StatusMenuManager smm;
	string curRoomName;
	bool toInit = false;
	List<string> registeredPermItems;
	List<Cutscene> currentCutscenes;

	void Awake () {
		if (manager == null) {
			Object.DontDestroyOnLoad (this);
			manager = this;
			initGame ();
		}else if  (manager != this) {
			Destroy (this);
		}
		currentCutscenes = new List<Cutscene> ();
		registeredPermItems = new List<string> ();
		SceneManager.sceneLoaded += initRoom;
	}
	void Start() {
		smm = GetComponent<StatusMenuManager> ();
		//initRoom (null,null);
	}

	void initGame() {
		if (mSaves == null) {
			mSaves = new SaveObjManager ();
			mSaves.resetRoomData ();
		}
	}
	void initRoom(Scene scene, LoadSceneMode mode) {
//		Debug.Log ("initRoom from game. Room:" + SceneManager.GetActiveScene ().name);
		GameObject[] obj = GameObject.FindGameObjectsWithTag ("jumpThru");

		foreach (GameObject go in obj) {
			go.AddComponent<JumpThru> ();
			EdgeCollider2D []ec = go.GetComponentsInChildren<EdgeCollider2D> ();
			foreach (EdgeCollider2D e in ec) {
				e.gameObject.AddComponent<JumpThru> ();
			}
		}
		foundPlayer = false;
		curRoomName = SceneManager.GetActiveScene ().name;
		bottomOfWorld = float.MinValue;
		Attackable[] atkM = FindObjectsOfType<Attackable> ();
		foreach (Attackable a in atkM) {
		//	a.bottomOfTheWorld = bottomOfWorld;
		}
		mSaves.onRoomLoad (curRoomName);
		curPlayer = FindObjectOfType<Player> ().gameObject;
		cameraInit ();
//		Debug.Log ("Done with init room");
	}
	void cameraInit() {
		GameObject camGO = FindObjectOfType<Camera> ().gameObject;
		if (camGO.GetComponent<CameraFollow> () == null) { 
			cf = camGO.AddComponent<CameraFollow> ();
		} else {
			cf = camGO.GetComponent<CameraFollow> ();
		}
		cf.focusAreaSize = new Vector2 (2f, 3f);
		cf.verticalOffset = 2f;
		cf.lookAheadDstX = 2f;
		cf.lookSmoothTimeX = 1f;
		cf.target = curPlayer.GetComponent<Movement>();
		camGO.GetComponent<Camera> ().orthographicSize = 10f;
		EdgeCollider2D[] edges = FindObjectsOfType<EdgeCollider2D> ();
		Vector2 minVertex = new Vector2 (float.MaxValue, float.MaxValue);
		Vector2 maxVertex = new Vector2 (-float.MaxValue, -float.MaxValue);
		foreach (EdgeCollider2D e in edges) {
			Vector2[] points = e.points;
			foreach (Vector2 v2 in points) {
				minVertex.x = Mathf.Min (minVertex.x, e.gameObject.transform.position.x +( e.transform.position.x + v2.x)/16f);
				minVertex.y = Mathf.Min (minVertex.y, e.gameObject.transform.position.y +( e.transform.position.y + v2.y)/16f);
				maxVertex.x = Mathf.Max (maxVertex.x, e.gameObject.transform.position.x +( e.transform.position.x + v2.x)/16f);
				maxVertex.y = Mathf.Max (maxVertex.y, e.gameObject.transform.position.y +( e.transform.position.y + v2.y)/16f);
				bottomOfWorld = Mathf.Min (bottomOfWorld, e.gameObject.transform.position.x + (e.transform.position.x + v2.x) / 16f - 15f);
				/*minVertex.x = Mathf.Min (minVertex.x, (v2.x)/20f);
				minVertex.y = Mathf.Min (minVertex.y, (v2.y)/20f);
				maxVertex.x = Mathf.Max (maxVertex.x, (v2.x)/20f);
				maxVertex.y = Mathf.Max (maxVertex.y, (v2.y)/20f);*/
			}
		}
		cf.maxVertex = maxVertex;
		cf.minVertex = minVertex;
		cf.initFunct();
	}
		
	// Update is called once per frame
	void Update () {
		/*if (Input.GetKeyDown(KeyCode.Escape)) {
			Debug.Log (SceneManager.GetActiveScene ().name);
			//SceneManager.LoadScene ("MainMenu", LoadSceneMode.Single);
		}*/
		if (Input.GetButtonDown("Debug")) {
			setDebug (!debug);
		}
		foreach (Cutscene c in currentCutscenes) {
			c.cutsceneUpdate (Time.deltaTime);
		}
	}

	public void setDebug(bool debugActive) {
		Debug.Log ("Switching debug mode to: " +debug);
		debug = debugActive;
		smm.setDebug (debug);
	}
	public void toggleMenu() {
		smm.toggleMenu ();
	}

	public void moveItem(GameObject gm,string newRoom, Vector3 newPos) {
		Debug.Log ("Moving item to " + newRoom + " at position: " + newPos);
		gm.GetComponent<PersItem> ().targetID = null;
		gm.GetComponent<Character> ().StoreData ();
		mSaves.moveItem (gm.GetComponent<Character> ().data, newRoom, newPos);
	}
	public void moveItem(GameObject gm,string newRoom, string newID,string newDir) {
		Debug.Log ("Moving item to " + newRoom + " at position: " + newID + " direction: " + newDir);
		Debug.Log (gm);
		Debug.Log(gm.GetComponent<Character>());
		gm.GetComponent<Character> ().StoreData ();
		mSaves.moveItem (gm.GetComponent<Character> ().data, newRoom, newID,newDir);
	}

	public void loadRoom(string name) {
		SceneManager.LoadScene (name, LoadSceneMode.Single);
	}
	public bool checkRegistered(GameObject go) {
		string id = go.name + "-" + SceneManager.GetActiveScene ().name + go.transform.position.ToString ();
		Character c = go.GetComponent<Character> ();
		//Debug.Log ("incoming ID: " + c.data.regID);
		if (c.data.regID != "Not Assigned") {
			id = c.data.regID;
		}
		//Debug.Log ("check registered: " + id);
		if (registeredPermItems.Contains(id) ){
			if (c.recreated) {
				//Debug.Log ("Recreated entity.");
				c.recreated = false;
				return false;
			} else {
				//Debug.Log ("already registered, removing");
				return true;
			}
		}
		//Debug.Log ("new entity. Adding to registry");
		registeredPermItems.Add(id);
		c.data.regID = id;
		//Debug.Log ("saved ID is: " + c.data.regID);
		return false;
	}
	public void addCutscene(Cutscene c) {
		currentCutscenes.Insert (0, c);
	}
	public void concludeCutscene(Cutscene c) {
		currentCutscenes.Remove (c);
	}
}
