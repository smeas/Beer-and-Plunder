using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactables
{
	public class Beer : MonoBehaviour, IInteractable
	{
		[SerializeField]
		private BeerData beerData;

		public void Interact()
		{
			throw new System.NotImplementedException();
		}
		public void CancelInteraction()
		{
			throw new System.NotImplementedException();
		}
	}
}
