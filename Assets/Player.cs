using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof(EntityController))]
[RequireComponent(typeof(Interactor))]
public class Player : MonoBehaviour {
	public enum PlayerNumber {P1, P2};
	public PlayerNumber playerNumber;
	private string controlSuffix;

	private EntityController controller;
	private Interactor interactor;

	void Start() {
		controller = GetComponent<EntityController>();
		interactor = GetComponent<Interactor>();
		controlSuffix = playerNumber.ToString ();
	}

	void Update() {
		if (Input.GetButtonDown ("Pickup" + controlSuffix)) {
			controller.animator.SetBool ("Pickup", true);
			interactor.Pickup ();
		}

		bool justInteracted = Input.GetButtonDown ("Interact" + controlSuffix);
		bool interacting = Input.GetButton ("Interact" + controlSuffix);
		if (justInteracted || interacting) {
			bool didInteract = interactor.Interact (justInteracted, interacting);
			controller.animator.SetBool ("Interact", didInteract);
		} else {
			controller.animator.SetBool ("Interact", false);
		}


		Vector3 moveDirection = new Vector3 (Input.GetAxis ("Horizontal" + controlSuffix), 0, Input.GetAxis ("Vertical" + controlSuffix));
		bool run = Input.GetButton ("Run" + controlSuffix);
		controller.Move (moveDirection, run);
	}
}
