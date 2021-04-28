using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vikings {
	public class VikingHandAttack : MonoBehaviour
	{
		private Viking viking;

		private void Start() {
			viking = GetComponentInParent<Viking>();
		}

		private void OnCollisionEnter(Collision collision) {
			Debug.Log("Collision");
			if (collision.gameObject.CompareTag("Player")) {
				PlayerComponent player = collision.gameObject.GetComponent<PlayerComponent>();
				player.TakeBrawlDamage(viking.Data.attackDamage);
			}
		}

		private void OnTriggerEnter(Collider other) {
			Debug.Log("Trigger");
			if (other.gameObject.CompareTag("Player")) {
				PlayerComponent player = other.gameObject.GetComponent<PlayerComponent>();
				player.TakeBrawlDamage(viking.Data.attackDamage);
			}
		}
	}
}
