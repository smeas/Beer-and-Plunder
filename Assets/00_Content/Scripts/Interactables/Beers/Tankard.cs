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
    
		private void FixedUpdate() {
			if (isFull && Vector3.Dot(transform.up, Vector3.down) >= -0.2f)
				Spill();
		}
    
		public override void RoundOverReset() {
			base.RoundOverReset();
			//Empties out beer tankards between rounds
			IsFull = false;
		}

		protected override void OnPlace() {
			AudioManager.PlayEffectSafe(SoundEffect.Physics_TankardPlace);
		}

		private void Spill() {
			IsFull = false;
			AudioManager.PlayEffectSafe(SoundEffect.Physics_SpillBeer);
		}
	}
}