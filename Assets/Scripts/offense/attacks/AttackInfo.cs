using System;
using UnityEngine;

public class AttackInfo : MonoBehaviour {
	public string attackName = "default";

	public bool CreateHitbox = true;
	public Vector2 HitboxScale = new Vector2 (1.0f, 1.0f);
	public Vector2 HitboxOffset = new Vector2(0f,0f);
	public bool UniqueAIPrediction = false;
	public Vector2 AIPredictionHitbox = Vector2.zero;
	public Vector2 AIPredictionOffset = Vector2.zero;

	public float damage = 10.0f;
	public float stun = 0.3f;
	public float hitboxDuration = 0.5f;
	public Vector2 knockback = new Vector2(10.0f,10.0f);
	public float startUpTime = 0.5f;
	public float recoveryTime = 1.0f;
	public string StartUpAnimation = "none";
	public string RecoveryAnimation = "none";
	public float animSpeed = 1f;
	public string hitType = "melee";

	[HideInInspector]
	public float timeSinceStart = 0.0f;

	public GameObject attackFX;
	public AudioClip startupSoundFX;
	public AudioClip attackSoundFX;

	protected Fighter fighter;
	protected Attackable attackable;

	void Start () {
		fighter = GetComponent<Fighter> ();
		attackable = GetComponent<Attackable> ();
		if (!UniqueAIPrediction){
			AIPredictionHitbox = HitboxScale;
			AIPredictionOffset = HitboxOffset;
		}
	}

	// Update is called once per frame
	void Update () {}

	public virtual void onStartUp() {}

	public virtual void onAttack() {}

	public virtual void onConclude() {}

	public virtual void startUpTick() {}

	public virtual void recoveryTick() {}

	public virtual void onHitConfirm(GameObject other) {}

	public virtual void onInterrupt(float stunTime, bool successfulHit, Hitbox hb) {}

	public void onDrawGizmos() {

	}
}

