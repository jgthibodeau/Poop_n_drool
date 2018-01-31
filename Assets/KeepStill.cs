using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepStill : MonoBehaviour {
	public enum Type {World, Local};

	public bool keepPosition;
	public Type positonType;

	public bool keepRotation;
	public Type rotationType;

	public Vector3 originalWorldPosition;
	public Quaternion originalWorldRotation;

	public Vector3 originalLocalPosition;
	public Quaternion originalLocalRotation;

	// Use this for initialization
	void Start () {
		originalWorldPosition = transform.position;
		originalWorldRotation = transform.rotation;

		originalLocalPosition = transform.localPosition;
		originalLocalRotation = transform.localRotation;
	}
	
	// Update is called once per frame
	void Update () {
		if (keepPosition) {
			switch (positonType) {
			case Type.Local:
				transform.position = transform.parent.position + originalLocalPosition;
				break;
			case Type.World:
				transform.position = originalWorldPosition;
				break;
			}
		}
		if (keepRotation) {
			switch (rotationType) {
			case Type.Local:
				transform.localRotation = originalLocalRotation;
				break;
			case Type.World:
				transform.rotation = originalWorldRotation;
				break;
			}
		}
	}
}
