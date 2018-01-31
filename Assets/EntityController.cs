using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
//[RequireComponent(typeof(Rigidbody))]
public class EntityController : MonoBehaviour {
	public float speed = 6.0f;
	public float runSpeed = 9.0f;
	public float maxDirectTurnVelocity = 1f;
	public float drag = 0.5f;

	public bool grounded = false;

	private Rigidbody rb;
	public Animator animator;

	public float isGroundedRayLength = 0.1f;
	public LayerMask groundLayer;

	private Vector3 moveDirection = Vector3.zero;
	private bool running = false;

	void Start() {
		animator.SetBool("Grounded", true);
	}

	public void Move(Vector3 moveDirection, bool run) {
		if (moveDirection.magnitude > 1) {
			moveDirection.Normalize ();
		}
		this.moveDirection = moveDirection * speed;
		this.running = run;
	}

	public void Stop() {
		this.moveDirection = Vector3.zero;
	}

	void FixedUpdate() {
		grounded = isGrounded;
		animator.SetBool("Grounded", grounded);

		if (rb == null) {
			rb = GetComponent<Rigidbody>();
		}

		if (rb != null) {
			if (moveDirection.magnitude > 0) {
				moveDirection.Normalize ();

				Vector3 lookAt = transform.position;
				if (rb.velocity.magnitude > maxDirectTurnVelocity) {
					lookAt += rb.velocity;
				} else {
					lookAt += moveDirection;
				}
				lookAt.y = transform.position.y;
				transform.LookAt (lookAt);

				if (running) {
					moveDirection *= runSpeed;
				} else {
					moveDirection *= speed;
				}

				rb.AddForce (moveDirection, ForceMode.VelocityChange);
			}

//			if (grounded) {
				Vector3 vel = rb.velocity;
				vel.x *= drag;
				vel.z *= drag;
				rb.velocity = vel;
//			}

			animator.SetFloat ("MoveSpeed", rb.velocity.magnitude);
		} else {
			animator.SetFloat ("MoveSpeed", 0);
		}
	}

	public bool isGrounded {
		get {
			Vector3 position = transform.position;
			position.y = GetComponent<Collider>().bounds.min.y + 0.1f;
			float length = isGroundedRayLength + 0.2f;
			Debug.DrawRay (position, Vector3.down * length);
			return Physics.Raycast (position, Vector3.down, length, groundLayer);
		}
	}
}
