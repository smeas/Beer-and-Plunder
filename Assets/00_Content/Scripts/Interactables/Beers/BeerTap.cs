using Extensions;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

namespace Interactables.Beers {

	public class BeerTap : Interactable {

		[SerializeField] private GameObject beerPrefab;
		[SerializeField] private Transform beerSpawnpoint;
		[SerializeField] private InputActionAsset asset;
		[SerializeField] private Image progressBarImage;
		[SerializeField] private GameObject progressBar;

		private ItemSlot itemSlot;
		private float pouringProgress = 0;
		InputAction inputActionInteract;
		ButtonControl buttonInteract;

		private void Start() {
			itemSlot = GetComponentInChildren<ItemSlot>();
			inputActionInteract = asset.FindAction("Interact");
			buttonInteract = (ButtonControl)inputActionInteract.controls[0];
		}

		public override void Interact() {

			if (beerSpawnpoint == null) {
				Debug.LogError("No spawnpoint for beer on beerTap");
				return;
			}

			if (itemSlot.HasItemInSlot) {
				Debug.Log("Can´t pour, has full beer in slot");
				return;
			}

			StartCoroutine(PourBeer());
		}

		public override void CancelInteraction() {
			base.CancelInteraction();
		}

		private IEnumerator PourBeer() {

			while (!itemSlot.HasItemInSlot && buttonInteract.isPressed && pouringProgress <= 100) {

				pouringProgress++;

				if(!progressBar.activeInHierarchy)
					progressBar.SetActive(true);

				progressBarImage.fillAmount = pouringProgress * 0.01f;

				if(pouringProgress > 100) {
					Instantiate(beerPrefab, beerSpawnpoint.position, Quaternion.identity);
					pouringProgress = 0;
					progressBar.SetActive(false);
					break;
				}

				yield return null;
			}
			
			yield return 0;
		}

		
	}
}