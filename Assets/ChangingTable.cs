using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangingTable : Interactable {
	public GameObject dirtyDiaper;

	ItemHolder[] holders;
	Baby baby;
	Pickupable diaper;
	ItemHolder diaperHolder;

	private Tag.TagName[] cleanDiaperTags = {
		Tag.TagName.DIAPER, Tag.TagName.CLEAN
	};

	// Use this for initialization
	void Awake () {
		holders = GetComponents<ItemHolder> ();
	}

	public override bool IsInteractable (Interactor interactor, ItemHolder itemHolder) {
		//this has a poopy baby and a clean diaper

		foreach (ItemHolder holder in holders) {
			if (holder.HasItem ()) {
				if (holder.heldItem.HasTags (cleanDiaperTags)) {
					diaper = holder.heldItem;
					diaperHolder = holder;
				} else if (holder.heldItem.HasTag (Tag.TagName.BABY)) {
					baby = holder.heldItem.GetComponent<Baby> ();
				}
			}
		}

		return diaper != null && baby != null && baby.isPoopy;
	}

	public override void DoInteract (Interactor interactor, ItemHolder itemHolder, bool started, bool continued) {
//		SetTakable (false);
		baby.Change ();
		if (!baby.isPoopy) {
//			SetTakable (true);
			diaper.DestroyItem ();
			SpawnDirtyDiaper (diaperHolder);
		}
	}

	private void SpawnDirtyDiaper(ItemHolder itemHolder) {
		GameObject newItem = GameObject.Instantiate (dirtyDiaper);
		itemHolder.Pickup (newItem.GetComponent<Pickupable> ());
		newItem.transform.localPosition = itemHolder.heldLocation.localPosition;
		newItem.transform.localRotation = itemHolder.heldLocation.localRotation;
	}

	private void SetTakable(bool takable) {
		foreach (ItemHolder holder in holders) {
			holder.takable = takable;
		}
	}
}
