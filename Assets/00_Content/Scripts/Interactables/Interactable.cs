using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Interactables {
	public class Interactable : MonoBehaviour, IInteractable {

		public Action onInteraction;
		public Action onInteractionCancelled;

		public void CancelInteraction() {
			onInteractionCancelled?.Invoke();
		}

		public void Interact() {
			onInteraction?.Invoke();
		}
	}
}
