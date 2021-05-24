using Audio;
using Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Taverns {

	public class Entrance : MonoBehaviour
	{
		[SerializeField] private LayerMask vikingLayer;

		private void OnTriggerEnter(Collider other) {
			if (vikingLayer.ContainsLayer(other.gameObject.layer)) {
				AudioManager.PlayEffectSafe(SoundEffect.Viking_Desire_NeedTable);
			}
		}
	}	
}
