﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickupable : Tag {
	public bool removeRigidbody;
	public bool destroyable = true;
	public Vector3 heldLocalRotation;
	public ItemHolder itemHolder;

	private Rigidbody originalRb;
	private RigidbodyConstraints originalConstraints;
	private float mass, drag, angularDrag;
	private bool useGravity, isKinematic;
	private RigidbodyInterpolation interpolation;
	private CollisionDetectionMode collisionDetectionMode;
	public Collider[] colliders;

	private float pickupSpeed = 15.0f;
	private float pickupRotateSpeed = 10f;

	private int defaultLayer;
	void Awake () {
		originalRb = GetComponentInParent<Rigidbody> ();
		int layer = LayerMask.NameToLayer ("Pickupable");
		defaultLayer = LayerMask.NameToLayer ("Default");
		MoveToLayer (transform, layer);
		if (itemHolder != null) {
			itemHolder.Pickup (this);
		}
	}

	void MoveToLayer (Transform root, int layer) {
		if (root.gameObject.GetComponent<Collider> () != null && root.gameObject.layer == defaultLayer) {
			root.gameObject.layer = layer;
		}
		foreach (Transform t in root) {
			MoveToLayer (t, layer);
		}
	}

	void Update() {
		if (IsHeld ()) {
			Freeze ();
		} else {
			UnFreeze ();
		}
	}

	public void Pickup (ItemHolder itemHolder) {
		if (this.itemHolder != null) {
			this.itemHolder.Drop ();
		}

		this.itemHolder = itemHolder;
		transform.parent = itemHolder.heldLocation;

		if (removeRigidbody) {
			Rigidbody rb = GetComponent<Rigidbody> ();
			originalConstraints = rb.constraints;
			mass = rb.mass;
			drag = rb.drag;
			angularDrag = rb.angularDrag;
			useGravity = rb.useGravity;
			isKinematic = rb.isKinematic;
			interpolation = rb.interpolation;
			collisionDetectionMode = rb.collisionDetectionMode;
			Destroy (rb);
		} else {
			originalConstraints = originalRb.constraints;
			originalRb.constraints = RigidbodyConstraints.FreezeAll;
			colliders = GetComponentsInChildren <Collider> ();
			for (int i = 0; i < colliders.Length; i++) {
				Collider c = colliders [i];
				if (c.isTrigger) {
					colliders [i] = null;
				} else {
					c.isTrigger = true;
				}
			}
		}
	}

	public void Drop() {
		transform.parent = null;
		itemHolder = null;

		if (removeRigidbody) {
			Rigidbody rb = gameObject.AddComponent<Rigidbody> ();
			rb.constraints = originalConstraints;
			rb.mass = mass;
			rb.drag = drag;
			rb.angularDrag = angularDrag;
			rb.useGravity = useGravity;
			rb.isKinematic = isKinematic;
			rb.interpolation = interpolation;
			rb.collisionDetectionMode = collisionDetectionMode;
		} else {
			originalRb.constraints = originalConstraints;
			for (int i = 0; i < colliders.Length; i++) {
				Collider c = colliders [i];
				if (c != null) {
					c.isTrigger = false;
				}
			}
		}
	}

	void Freeze() {
		if (Vector3.SqrMagnitude (transform.localPosition - Vector3.zero) > 0.1f) {
			transform.localPosition = Vector3.Slerp (transform.localPosition, Vector3.zero, Time.deltaTime * pickupSpeed);
		} else {
			transform.localPosition = Vector3.zero;
		}

		if (Vector3.Angle (transform.localEulerAngles, Vector3.zero) > 5f) {
			transform.localRotation = Quaternion.Slerp (transform.localRotation, Quaternion.identity, Time.deltaTime * pickupRotateSpeed);
		} else {
			transform.localRotation = Quaternion.identity;
		}
	}

	void UnFreeze() {
	}

	public void DestroyItem() {
		Drop ();
		if (destroyable) {
			GameObject.Destroy (gameObject);
		}
	}

	public bool IsHeld() {
		return (itemHolder != null);
	}

	public bool IsPickupable() {
		return itemHolder == null || itemHolder.takable;
	}
}
