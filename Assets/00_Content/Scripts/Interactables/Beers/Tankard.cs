using Audio;
using System.Collections;
using UnityEngine;
using Vikings;

namespace Interactables.Beers {
	public class Tankard : PickUp, IDesirable {
		[SerializeField] private BeerData beerData;
		[SerializeField] private GameObject foam;

		//Var for ensuring the foam fades away in a nice gradual way when a new round starts
		private float shrinkSpeed = 0.5f;
		private float shrinkSize = 1f;
		private float shrinkThreshold = 0.1f;
		private Vector3 foamStartingSize;

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
			//Saves this to reset it before the tankard is filled again
			foamStartingSize = foam.transform.localScale;
		}

		private void FixedUpdate() {
			if (isFull && Vector3.Dot(transform.up, Vector3.down) >= -0.2f)
				Spill();
		}

		public override void HandleNewRoundReset() {
			base.HandleNewRoundReset();

			if (foam != null && IsFull == true) StartCoroutine(CoEmptyOutTankards());
		}

		private IEnumerator CoEmptyOutTankards() {
			//Gradually shrinks the foam of the tankard until it is below the threshold and then sets the tankard to empty
			while(IsFull == true) {
				foam.transform.localScale -= Vector3.one * Time.deltaTime * shrinkSpeed;
				shrinkSize -= 1f * Time.deltaTime * shrinkSpeed;

				if (shrinkSize <= shrinkThreshold) {
					shrinkSize = 1f;
					IsFull = false;
					//Resets the scale of the foam now that it has been set to inactive by the bool IsFull
					foam.transform.localScale = foamStartingSize;
				}

				yield return null;
			}
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