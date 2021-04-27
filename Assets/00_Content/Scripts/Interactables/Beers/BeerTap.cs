using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Taverns;

namespace Interactables.Beers {

	public class BeerTap : Interactable {

		[Header("Settings")]
		[Range(1, 100f)]
		[SerializeField] private float pourTimeMultiplier = 10f;

		[Header("GameObjects")]
		[SerializeField] private GameObject beerPrefab;
		[SerializeField] private Image progressBarImage;
		[SerializeField] private GameObject progressBar;
		//I added a field here for the beerData SO, so that I could get the cost from it.
		[SerializeField] private BeerData beerData;

		private ItemSlot itemSlot;
		private float pouringProgress = 0;
		private bool isHolding = false;

		private void Start() {
			itemSlot = GetComponentInChildren<ItemSlot>();
		}

		public override void Interact(GameObject player, PickUp item) {

			isHolding = true;

			if (itemSlot.HasItemInSlot)
				return;

			StartCoroutine(PourBeer());
		}

		public override void CancelInteraction() {
			isHolding = false;
		}

		private IEnumerator PourBeer() {

			if (Tavern.Instance.Money >= beerData.cost) {
				//I added that the tavern singleton must contain more money than the cost of the beer in order for the player to be able to pour the beer at all.
				while (!itemSlot.HasItemInSlot && isHolding && pouringProgress <= 100 && Tavern.Instance.Money >= beerData.cost) {

					pouringProgress += pourTimeMultiplier * Time.deltaTime;

					if (!progressBar.activeInHierarchy)
						progressBar.SetActive(true);

					progressBarImage.fillAmount = pouringProgress * 0.01f;

					if (pouringProgress > 100) {
						GameObject beer = Instantiate(beerPrefab);
						itemSlot.PutItem(beer.GetComponent<PickUp>());

						//Added a line of code so that the player draws the cost just as the beer is done.
						Tavern.Instance.SpendsMoney(beerData.cost);

						pouringProgress = 0;
						progressBar.SetActive(false);
						break;

					}

					yield return null;
				}
			} else {
				Debug.Log("You don't have enough money to pour a beer!");
			}

		}
	}
}
