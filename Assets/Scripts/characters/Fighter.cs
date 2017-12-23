using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (HitboxMaker))]
[RequireComponent (typeof (Movement))]

public class Fighter : MonoBehaviour {
	public float recoveryTime = 0.0f;
	public float startUpTime = 0.0f;
	public float stunTime = 0.0f;
	public int hitCombo = 0;
	public Dictionary<string,AttackInfo> attacks = new Dictionary<string,AttackInfo>();

	public string AnimAir = "air";
	public string AnimRun = "run";
	public string AnimIdle = "idle";
	public string AnimHit = "hit";

	string myFac;
	Movement movement;
	AnimatorSprite m_anim;
	Attackable attackable;
	GameManager gameManager;
	HitboxMaker hbm;
	AttackInfo currentAttack;
	public string currentAttackName;
	bool hitboxCreated;
	public bool reflectProj;
	float maxStun;
	float animationRatio;
	bool startingNewAttack;

	public AudioClip AttackSound;

	// Use this for initialization
	void Start () {
		m_anim = GetComponent<AnimatorSprite> ();
		movement = GetComponent<Movement> ();
		gameManager = FindObjectOfType<GameManager> ();
		attackable = GetComponent<Attackable> ();
		myFac = gameObject.GetComponent<Attackable> ().faction;
		hbm = GetComponent<HitboxMaker> ();
		endAttack ();
		AttackInfo[] at = gameObject.GetComponents<AttackInfo> ();
		foreach (AttackInfo a in at) {
			if (!attacks.ContainsKey(a.attackName))
				attacks.Add (a.attackName, a);
		}
		animationRatio = 1f;
	}

	// Update is called once per frame
	void Update () {
		startingNewAttack = false;
		if (stunTime > 0.0f ) {
			m_anim.Play (AnimHit);

			stunTime = Mathf.Max (0.0f, stunTime - Time.deltaTime);
			if (stunTime == 0.0f && attackable.alive) {
				endStun ();
			}
		}else if (!attackable.alive) {
			startHitState (3.0f);
		}else if (currentAttackName != "none") {
			currentAttack.timeSinceStart = currentAttack.timeSinceStart + Time.deltaTime;
			currentAttack.startUpTick ();

			if (hitboxCreated == false) {
				if (startUpTime <= (Time.deltaTime / 2)) {
					hitboxCreated = true;

					currentAttack.onAttack ();

					if (currentAttack.attackSoundFX != null) {AudioSource.PlayClipAtPoint (currentAttack.attackSoundFX, transform.position);}
					if (AttackSound != null) {
						AudioSource.PlayClipAtPoint (AttackSound, gameObject.transform.position);
					}
					if (currentAttack.attackFX) {
						addEffect (currentAttack.attackFX, currentAttack.recoveryTime + 0.2f);
					}

					if (currentAttack.CreateHitbox) {
						Vector2 realKB = currentAttack.knockback;
						Vector2 realOff = currentAttack.HitboxOffset;
						float damage = currentAttack.damage;
						float stun = currentAttack.stun;
						hbm.addAttrs (currentAttack.hitType);
						if (gameObject.GetComponent<Movement> ().facingLeft) {
							realKB = new Vector2 (-currentAttack.knockback.x, currentAttack.knockback.y);
							realOff = new Vector2 (-currentAttack.HitboxOffset.x, currentAttack.HitboxOffset.y);
						}
						hbm.hitboxReflect = reflectProj;
						hbm.stun = stun;
						hbm.createHitbox (currentAttack.HitboxScale, realOff, damage, currentAttack.hitboxDuration, realKB, true, myFac, true);
					}
					m_anim.Play (currentAttack.RecoveryAnimation);
				} else {
					startUpTime = Mathf.Max (0.0f, startUpTime - Time.deltaTime);
				}

			} else {
				if (recoveryTime <= Time.deltaTime / 2.0f) {
					endAttack ();
				} else {
					currentAttack.recoveryTick ();
					recoveryTime = Mathf.Max (0.0f, recoveryTime - Time.deltaTime);
				}
			}
		} else {
			StandardAnimation ();
		}
	}

	void addEffect(GameObject attackFX,float lifeTime) {
		GameObject fx = GameObject.Instantiate (attackFX, transform);
		fx.GetComponent<disappearing> ().duration = currentAttack.recoveryTime;

		fx.GetComponent<disappearing> ().toDisappear = true;
		fx.GetComponent<Follow> ().followObj = gameObject;
		fx.GetComponent<Follow> ().followOffset = new Vector3 (0.0f, 0.0f, -3.0f);
		fx.GetComponent<Follow> ().toFollow = true;
		if (movement.facingLeft) {
			fx.transform.Rotate (new Vector3 (0f, 180f,0f));
		}

		ParticleSystem [] partsys = fx.GetComponentsInChildren<ParticleSystem> ();
		foreach (ParticleSystem p in partsys) {
			ParticleSystem.MainModule mainP = p.main;
			mainP.startLifetime = lifeTime; 
		}
	}
	public bool isAttacking() {
		return (currentAttackName == "none");
	}

	public void registerStun(float st, bool defaultStun,Hitbox hb) {
		if (defaultStun) {
			startHitState (st);
		}
		if (currentAttack != null) {
			currentAttack.onInterrupt (stunTime,defaultStun,hb);
		}
	}
	void startHitState(float st) {
		//Debug.Log ("Starting Hit State with Stun: "+ st);
		endAttack ();
		if (stunTime > 0.0f) {
			hitCombo = hitCombo + 1;
		} else {
			hitCombo = 1;
		}
		stunTime = st;
		maxStun = st;
		movement.canMove = false;
	}

	public void registerHit(GameObject otherObj) {
		if (currentAttack != null) {
			currentAttack.onHitConfirm (otherObj);
			if (GetComponent<Player> ()) {
				GetComponent<Player> ().onHitConfirm(otherObj);
			}
			if (GetComponent<Character> ()) {
				EVHitConfirm e = new EVHitConfirm ();
				e.targetChar = GetComponent<Character> ();
				e.ObjectHit = otherObj;
				e.AttackData = currentAttack;
				GetComponent<Observable> ().broadcastToObservers (e);
				if (otherObj.GetComponent<Observer> ()) {
					otherObj.GetComponent<Observer> ().respondToEvent (e);		
				}
			}
		}
	}

	public void endStun() {
		if (attackable.alive) {
			movement.canMove = true;
			hbm.clearAttrs ();
			stunTime = 0.0f;
			maxStun = 0.0f;
			hitCombo = 0;
		}
	}
	public void endAttack() {
		if (currentAttack != null) {
			currentAttack.onConclude ();
			currentAttack.timeSinceStart = 0.0f;
		}
		if (startingNewAttack)
			return;
		currentAttackName = "none";
		startUpTime = 0.0f;
		recoveryTime = 0.0f;
		m_anim.SetSpeed (1.0f);
		hitboxCreated = false;
		currentAttack = null;
		reflectProj = false;
		movement.canMove = true;
	}
	public bool tryAttack(string[] attackList) {
		foreach (string s in attackList) {
			if (attacks.ContainsKey (s)) {
				tryAttack (s);
				return true;
			}
		}
		return false;
	}

	public bool tryAttack(string attackName) {
		if (currentAttackName == "none" && attacks.ContainsKey(attackName) && stunTime <= 0.0f) {
			hitboxCreated = false;
			currentAttackName = attackName;
			currentAttack = attacks[currentAttackName];
			startUpTime = (currentAttack.startUpTime) - (Time.deltaTime * 2);
			recoveryTime = currentAttack.recoveryTime;
			m_anim.Play (currentAttack.StartUpAnimation);
			m_anim.SetSpeed(currentAttack.animSpeed * animationRatio);
			movement.canMove = false;
			currentAttack.onStartUp ();
			currentAttack.timeSinceStart = 0.0f;
			startingNewAttack = true;
			if (currentAttack.startupSoundFX != null) {AudioSource.PlayClipAtPoint (currentAttack.startupSoundFX, transform.position);}
			if (GetComponent<Character> ()) {
				EVAttack e = new EVAttack ();
				e.targetChar = GetComponent<Character> ();
				e.AttackData = currentAttack;
				GetComponent<Observable> ().broadcastToObservers (e);
			}
			return true;
		}
		return false;
	}

	public void StandardAnimation() {
		if (!movement.onGround) {
			//Debug.Log ("Standard Anim Air");
			m_anim.Play (AnimAir);
		} else {
			if (movement.AttemptingMovement) {
				m_anim.Play (AnimRun);
			} else {
				m_anim.Play (AnimIdle);
			}
		}
	}
}
