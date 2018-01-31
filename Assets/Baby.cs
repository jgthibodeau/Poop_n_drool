using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(EntityController))]
[RequireComponent(typeof(Interactor))]
public class Baby : MonoBehaviour {
	private EntityController controller;
	private Interactor interactor;
	private Pickupable pickupable;

	[Header("Movement")]
	public float stayStillChance = 0.25f;
	public float stopDistance = 0.05f;
	public float maxDistance = 5;
	public float turnMinTime = 1;
	public float turnMaxTime = 3;
	public float remainingTurnTime = 0;

	public bool useNavMeshAgent = false;
	private Vector3 targetPosition;
	private NavMeshAgent agent;


	[Header("Puke")]
	public GameObject puke;
	public GameObject pukeTrail;
	public bool doPuke;
	public float pukeInterval;
	public float remainingPukeInterval;
	[Range(0, 1)]
	public float pukeChance;

	public Image dirtyIcon;
	public Image dirtyIconFill;
	public bool isDirty;
	public float cleanTime;
	public float remainingCleanTime;
	public float cleanSpeed;


	[Header("Poop")]
	public GameObject cleanDiaper;
	public GameObject dirtyDiaper;
	public bool doPoop;
	public float poopInterval;
	public float remainingPoopInterval;
	[Range(0, 1)]
	public float poopChance;

	public Image poopyIcon;
	public Image poopyIconFill;
	public bool isPoopy;
	public float changeTime;
	public float remainingChangeTime;
	public float changeSpeed;


	[Header("Food")]
	public bool doHunger;
	public float hungerInterval;
	public float remainingHungerInterval;
	[Range(0, 1)]
	public float hungerChance;

	public Image hungryIcon;
	public Image hungryIconFill;
	public bool isHungry;
	public float feedTime;
	public float remainingFeedTime;
	public float feedSpeed;


	[Header("Happiness")]
	public bool doSad;
	public float sadVelocity;
	public LayerMask sadColliders;
	public int sadHits;
	public int maxSadHits;

	public Image sadIcon;
	public Image sadIconFill;
	public bool isSad;
	public float cheerupTime;
	public float remainingCheerupTime;
	public float cheerupSpeed;


	[Header("Sleep")]
	public bool doSleep;
	public float sleepInterval;
	public float remainingSleepInterval;
	[Range(0, 1)]
	public float sleepChance;

	public Image sleepyIcon;
	public Image sleepyIconFill;
	public bool isSleepy;
	public float sleepTime;
	public float remainingSleepTime;
	public float sleepSpeed;


	void Start() {
		controller = GetComponent<EntityController>();
		interactor = GetComponent<Interactor>();
		pickupable = GetComponent<Pickupable>();
		agent = GetComponent<NavMeshAgent> ();

		remainingPukeInterval = pukeInterval;
		remainingPoopInterval = poopInterval;
		remainingHungerInterval = hungerInterval;
		remainingSleepInterval = sleepInterval;
	}

	void Update() {
		if (doPuke) {
			UpdateIcon (dirtyIcon, dirtyIconFill, cleanTime, remainingCleanTime, isDirty);
			remainingPukeInterval -= Time.deltaTime;
			if (remainingPukeInterval <= 0) {
				remainingPukeInterval = pukeInterval;
				if (Random.value <= pukeChance) {
					GameObject spawnedPuke = GameObject.Instantiate (puke);
					spawnedPuke.transform.position = new Vector3 (transform.position.x, transform.position.y + 0.1f, transform.position.z);
				}
			}
		}

		if (doPoop) {
			UpdateIcon (poopyIcon, poopyIconFill, changeTime, remainingChangeTime, isPoopy);
			remainingPoopInterval -= Time.deltaTime;
			if (remainingPoopInterval <= 0) {
				remainingPoopInterval = poopInterval;
				if (Random.value <= poopChance) {
					Poop ();
				}
			}
		}

		if (doHunger) {
			UpdateIcon (hungryIcon, hungryIconFill, feedTime, remainingFeedTime, isHungry);
			remainingHungerInterval -= Time.deltaTime;
			if (remainingHungerInterval <= 0) {
				remainingHungerInterval = hungerInterval;
				if (Random.value <= hungerChance) {
					Hunger ();
				}
			}
		}

		if (doSad) {
			UpdateIcon (sadIcon, sadIconFill, cheerupTime, remainingCheerupTime, isSad);
			if (isSad && pickupable.IsHeld () && pickupable.itemHolder.HasTag (Tag.TagName.PLAYER)) {
				Cheerup ();
			}
		}

		if (doSleep) {
			UpdateIcon (sleepyIcon, sleepyIconFill, sleepTime, remainingSleepTime, isSleepy);
			remainingSleepInterval -= Time.deltaTime;
			if (remainingSleepInterval <= 0) {
				remainingSleepInterval = sleepInterval;
				if (Random.value <= sleepChance) {
					Sleepy ();
				}
			}

			if (isSleepy && pickupable.IsHeld () && pickupable.itemHolder.HasTag (Tag.TagName.CRIB)) {
				Sleep ();
			}
		}

	}

	void UpdateIcon(Image icon, Image fill, float maxTime, float remainingTime, bool show) {
		if (show) {
			icon.gameObject.SetActive (true);
			fill.fillAmount = 1 - remainingTime / maxTime;
		} else {
			icon.gameObject.SetActive (false);
		}
	}

	void OnTriggerEnter(Collider collider) {
		if (collider.gameObject.tag == "Puke") {
			Dirty ();
		}
	}

	void OnTriggerStay(Collider collider) {
		if (collider.gameObject.tag == "Puke") {
			Dirty ();
		}
	}

	void OnCollisionEnter(Collision collision) {
		//if collision force strong enough, become sad
		if (collision.relativeVelocity.magnitude >= sadVelocity &&  (sadColliders == (sadColliders | (1 << collision.gameObject.layer)))) {
			Sad ();
		}
	}

	public void Dirty() {
		isDirty = true;
		remainingCleanTime = cleanTime;
		//TODO enable dirty particles
	}

	public void Clean() {
		remainingCleanTime -= Time.deltaTime * cleanSpeed;
		if (remainingCleanTime <= 0) {
			isDirty = false;
			//TODO disable dirty particles
		}
	}

	public void Poop() {
		isPoopy = true;
		remainingChangeTime = changeTime;

		cleanDiaper.SetActive (false);
		dirtyDiaper.SetActive (true);
	}

	public void Change() {
		remainingChangeTime -= Time.deltaTime * changeSpeed;
		if (remainingChangeTime <= 0) {
			isPoopy = false;
			cleanDiaper.SetActive (true);
			dirtyDiaper.SetActive (false);
		}
	}

	public void Hunger() {
		isHungry = true;
		remainingFeedTime = feedTime;
	}

	public void Feed() {
		remainingFeedTime -= Time.deltaTime * feedSpeed;
		if (remainingFeedTime <= 0) {
			isHungry = false;
		}
	}

	public void Sad() {
		sadHits++;
		if (sadHits >= maxSadHits) {
			isSad = true;
			remainingCheerupTime = cheerupTime;
		}
	}

	public void Cheerup() {
		remainingCheerupTime -= Time.deltaTime * cheerupSpeed;
		if (remainingCheerupTime <= 0) {
			isSad = false;
			sadHits = 0;
		}
	}

	public void Sleepy() {
		isSleepy = true;
		remainingSleepTime = sleepTime;
	}

	public void Sleep() {
		remainingSleepTime -= Time.deltaTime * sleepSpeed;
		if (remainingSleepTime <= 0) {
			isSleepy = false;
		}
	}

	void FixedUpdate() {
		if (pickupable.IsHeld ()) {
			remainingTurnTime = turnMaxTime;
			controller.Stop ();
			return;
		}

		if (controller.isGrounded) {
			if (remainingTurnTime <= 0) {
				if (Random.value > stayStillChance) {
					DecideTargetPosition ();
				}
				remainingTurnTime = Random.Range (turnMinTime, turnMaxTime);
			} else {
				remainingTurnTime -= Time.deltaTime;
			}

			if (useNavMeshAgent) {
				agent.SetDestination (targetPosition);
			} else {
				if (Vector3.Distance (transform.position, targetPosition) > stopDistance) {
					Vector3 moveDirection = targetPosition - transform.position;
					controller.Move (moveDirection, false);
				} else {
					controller.Stop ();
				}
			}
		}
	}

	void DecideTargetPosition() {
//		Vector3 rotation = new Vector3 (Random.Range (-1f, 1f), 0, Random.Range (-1f, 1f)).normalized;
//		targetPosition = transform.position + rotation * Random.Range (0, maxDistance);
		targetPosition = RandomNavmeshLocation (maxDistance);
	}

	public Vector3 RandomNavmeshLocation(float radius) {
		Vector3 randomDirection = Random.insideUnitSphere * radius;
		randomDirection += transform.position;
		NavMeshHit hit;
		Vector3 finalPosition = Vector3.zero;
		if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1)) {
			finalPosition = hit.position;            
		}
		return finalPosition;
	}
}
