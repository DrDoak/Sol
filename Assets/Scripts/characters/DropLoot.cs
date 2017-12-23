using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropLoot : MonoBehaviour {

	public GameObject healthPowerUp;
	public float chanceDropHealth;
	public GameObject energyPowerUp;
	public float chanceDropEnergy;
	public float chanceDropPowerupLg;
	public GameObject powerUpLg;
	public float chanceDropPowerupSm;
	public GameObject powerUpSm;
	void Start () {}
	void Update () {}

	void OnDestroy () {
		float rand = Random.value;
		if (rand * 100 < chanceDropHealth) {
			GameObject go = Instantiate(healthPowerUp,transform.position,Quaternion.identity) as GameObject; 
			go.GetComponent<Movement> ().addToVelocity (new Vector2 (Random.Range (-15, 15), 30));
		}
		rand = Random.value;
		if (rand * 100 < chanceDropEnergy) {
			GameObject go = Instantiate(energyPowerUp,transform.position,Quaternion.identity) as GameObject; 
			go.GetComponent<Movement> ().addToVelocity (new Vector2 (Random.Range (-15, 15), 30));
		}
		/*if (FindObjectOfType<Moon> () && FindObjectOfType<Moon> ().moonActive) {
			rand = Random.value;
			if (rand * 100 < chanceDropPowerupSm) {
				GameObject go = Instantiate (powerUpSm, transform.position, Quaternion.identity) as GameObject; 
				go.GetComponent<Movement> ().addToVelocity (new Vector2 (Random.Range (-15, 15), 30));
			}
			rand = Random.value;
			if (rand * 100 < chanceDropPowerupLg) {
				GameObject go = Instantiate (powerUpLg, transform.position, Quaternion.identity) as GameObject; 
				go.GetComponent<Movement> ().addToVelocity (new Vector2 (Random.Range (-15, 15), 30));
			}
		}*/
	}
}
