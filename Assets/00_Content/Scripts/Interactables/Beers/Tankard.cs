using DG.Tweening;
using Audio;
using System.Collections;
using UnityEngine;
using Vikings;

namespace Interactables.Beers {
	public class Tankard : PickUp, IDesirable {
		[SerializeField] private BeerData beerData;
		[SerializeField] private GameObject foam;
		[SerializeField] private float floorHitSoundVelocityLimit = 1f;

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
			if (IsFull && Vector3.Dot(transform.up, Vector3.down) >= -0.2f)
				Spill();
		}
		/// <summary>
		/// Tankards override on pickup, this shrinks away the foam on the tankard, instead of the whole tankard.
		/// </summary>
		public override void HandleNewRoundReset() {
			base.HandleNewRoundReset();

			if (foam != null && IsFull == true)
				foam.transform.DOScale(Vector3.zero, shrinkTime).OnComplete(() => {
					IsFull = false;
					foam.transform.localScale = foamStartingSize;
				});			
		}

		protected override void OnPlace() {
			AudioManager.PlayEffectSafe(SoundEffect.Physics_TankardPlace);
		}

		private void Spill() {
			IsFull = false;
			AudioManager.PlayEffectSafe(SoundEffect.Physics_SpillBeer);
		}

		private void OnCollisionEnter(Collision collision) {
			Debug.Log(collision.relativeVelocity.y);
			if (collision.gameObject.CompareTag("Ground") && collision.relativeVelocity.y > floorHitSoundVelocityLimit) {
				AudioManager.PlayEffectSafe(SoundEffect.BeerDrop);
			}
		}
	}
}