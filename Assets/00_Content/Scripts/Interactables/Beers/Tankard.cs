using Audio;
using UnityEngine;
using Vikings;

namespace Interactables.Beers {
	public class Tankard : PickUp, IDesirable {
		[SerializeField] private BeerData beerData;
		[SerializeField] private GameObject foam;

		private bool isFull;

		public DesireType DesireType => beerData.type;

		public bool IsFull {
			get => isFull;
			set {
				isFull = value;
				foam.SetActive(value);
			}
		}

		protected override void Start() {
			base.Start();
			IsFull = IsFull;
		}

		private void FixedUpdate() {
			if (IsFull && Vector3.Dot(transform.up, Vector3.down) >= -0.2f)
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

		private void OnCollisionEnter(Collision collision) {
			if (collision.gameObject.CompareTag("Ground") && collision.relativeVelocity.y > 4f) {
				AudioManager.PlayEffectSafe(SoundEffect.BeerDrop);
			}
		}
	}
}