using System.Linq;
using Audio;
using UnityEngine;
using Vikings;

namespace Interactables.Beers {
	public class Tankard : PickUp, IDesirable {
		[SerializeField] private BeerData beerData;
		[SerializeField] private Color emptyColor = Color.red;
		[SerializeField] private Color fullColor = new Color(1f, 0.6f, 0f);

		private bool isFull;

		public DesireType DesireType => beerData.type;

		public bool IsFull {
			get => isFull;
			set {
				isFull = value;

				// TODO: Replace this with something more efficient.
				foreach (Material material in GetComponentsInChildren<MeshRenderer>().SelectMany(x => x.materials))
					material.color = isFull ? fullColor : emptyColor;
			}
		}

		protected override void Start() {
			base.Start();
			IsFull = IsFull;
		}

		protected override void OnPlace() {
			AudioManager.PlayEffectSafe(SoundEffect.Physics_TankardPlace);
		}
	}
}