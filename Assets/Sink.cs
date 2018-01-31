using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sink : Interactable {
	ItemHolder babyHolder;
	Baby baby;

	// Use this for initialization
	void Awake () {
		babyHolder = GetComponent<ItemHolder> ();
	}

	protected override void Update(){
		base.Update ();
		if (babyHolder.HasItem ()) {
			baby = babyHolder.heldItem.GetComponent<Baby> ();
		} else {
			baby = null;
		}
	}

	public override bool IsInteractable (Interactor interactor, ItemHolder itemHolder) {
		return itemHolder.IsEmpty () && baby != null && baby.isDirty;
	}

	public override void DoInteract (Interactor interactor, ItemHolder itemHolder, bool started, bool continued) {
//		babyHolder.takable = false;
		baby.Clean ();
//		if (!baby.isDirty) {
//			babyHolder.takable = true;
//		}
	}
}
