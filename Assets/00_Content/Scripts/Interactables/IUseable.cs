using UnityEngine;

namespace Interactables {
	public interface IUseable {

		void Use(GameObject user);
		void EndUse();
	}
}
