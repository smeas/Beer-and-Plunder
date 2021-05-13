
using UnityEngine;

namespace Interactables.Kitchens {
	public class KitchenTicket : PickUp {
		[SerializeField] private float upVelocity;
		[SerializeField, Min(0)]
		private float minSpawnVelocity;
		[SerializeField, Min(0)]
		private float maxSpawnVelocity;

		protected override void Start() {

			Rigidbody rb = GetComponent<Rigidbody>();

			float x = Mathf.Sin(Random.Range(0, Mathf.PI * 2));
			float z = Mathf.Cos(Random.Range(0, Mathf.PI * 2));

			Vector3 direction = new Vector3(x, 0, z).normalized * Random.Range(minSpawnVelocity, maxSpawnVelocity);
			direction.y = upVelocity;

			rb.AddForce(direction, ForceMode.Impulse);
			rb.AddRelativeTorque(direction, ForceMode.Impulse);

			base.Start();
		}
	}
}
