using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puke : MonoBehaviour {
	Vector3 originalScale;

	public Transform soapOrigin;
	public Transform[] models;

	public float growSpeed = 1f;

	public float requiredSwipes;
	public float remainingSwipes;

	public AudioClip[] audioClips;
	public float minPitch = 1.5f;
	public float maxPitch = 1.5f;
	public float volume = 1f;

	public GameObject soap;
	public float soapLifeTime;

	void Start() {
		originalScale = transform.localScale;
		remainingSwipes = requiredSwipes;
		StartCoroutine ("Grow");

		float step = 0.0f;
		foreach (Transform t in models) {
			Vector3 modelRot = t.transform.eulerAngles;
			modelRot.y = Random.Range (step, step + 120.0f);
			t.eulerAngles = modelRot;
			step += 120f;
		}
	}

	IEnumerator Grow() {
		float scaleFactor = 0.25f;
		while (scaleFactor < 1f) {

			Vector3 newScale = transform.localScale;
			newScale.x *= scaleFactor;
			newScale.z *= scaleFactor;

			transform.localScale = newScale;
			scaleFactor += growSpeed * Time.deltaTime;
			yield return null;
		}
		transform.localScale = originalScale;
	}

	void Swipe() {
		CreateSound ();
		CreateSoap ();

		remainingSwipes--;
		if (remainingSwipes <= 0) {
			Destroy (gameObject);
		} else {
//			Vector3 newScale = originalScale * (1 - (requiredSwipes - remainingSwipes) / requiredSwipes);
//			transform.localScale = newScale;
			float scaleFactor = 1 - 1/requiredSwipes;
			Vector3 newScale = transform.localScale;
			newScale.x *= scaleFactor;
			newScale.z *= scaleFactor;
			transform.localScale = newScale;
		}
	}

	void CreateSound() {
		GameObject newSource = new GameObject ();
		newSource.transform.position = this.transform.position;
		AudioSource source = newSource.AddComponent<AudioSource> ();
		source.pitch = Random.Range (minPitch, maxPitch);
		AudioClip clip = audioClips [Random.Range (0, audioClips.Length)];
		source.PlayOneShot (clip, volume);

		Kill kill = newSource.AddComponent<Kill> ();
		kill.lifeTimeInSeconds = clip.length;
	}

	void CreateSoap() {
		GameObject newSoap = Instantiate (soap);
		newSoap.transform.position = soapOrigin.position;

		Kill kill = newSoap.AddComponent<Kill> ();
		kill.lifeTimeInSeconds = soapLifeTime;
	}

	void OnTriggerEnter(Collider collider) {
		Debug.Log ("puke "+collider.gameObject.tag);
		if (collider.gameObject.tag == "Mop") {
			Swipe ();
		}
	}
}
