using UnityEngine;
using Vikings;

namespace Interactables.Beers {
	public class Beer : PickUp, IDesirable {

		[SerializeField] private BeerData beerData;

		public DesireType DesireType => beerData.type;
	}
}
