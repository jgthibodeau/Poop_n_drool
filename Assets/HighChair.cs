using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighChair : Interactable {
	public GameObject dirtyBottle;

	ItemHolder babyHolder;
	Baby baby;

	private Tag.TagName[] fullBottleTags = {
		Tag.TagName.BOTTLE, Tag.TagName.FULL
	};

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
		return itemHolder.HasItem () && itemHolder.heldItem.HasTags (fullBottleTags) && baby != null && baby.isHungry;
	}

	public override void DoInteract (Interactor interactor, ItemHolder itemHolder, bool started, bool continued) {
		baby.Feed ();

		if (!baby.isHungry) {
			itemHolder.DestroyItem ();
			SpawnDirtyBottle (itemHolder);
		}
	}

	private void SpawnDirtyBottle(ItemHolder itemHolder) {
		GameObject newItem = GameObject.Instantiate (dirtyBottle);
		itemHolder.Pickup (newItem.GetComponent<Pickupable> ());
		newItem.transform.localPosition = itemHolder.heldLocation.localPosition;
		newItem.transform.localRotation = itemHolder.heldLocation.localRotation;
	}
}