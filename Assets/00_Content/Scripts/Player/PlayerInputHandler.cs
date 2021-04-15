using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Player {
	public class PlayerInputHandler : MonoBehaviour {
		[SerializeField] private UnityEvent<Vector2> onMove;
		[SerializeField] private UnityEvent onUse;
		[SerializeField] private UnityEvent onEndUse;
		[SerializeField] private UnityEvent onPickup;
		[SerializeField] private UnityEvent onDrop;
		[SerializeField] private UnityEvent onInteract;

		public UnityEvent<Vector2> OnMove => onMove;
		public UnityEvent OnUse => onUse;
		public UnityEvent OnEndUse => onEndUse;
		public UnityEvent OnPickup => onPickup;
		public UnityEvent OnDrop => onDrop;
		public UnityEvent OnInteract => onInteract;

		// These methods are invoked by the UnityEvents on the PlayerInput component.
		#region Input Action Handlers

		public void OnMoveInput(InputAction.CallbackContext ctx) {
			if (ctx.performed)
				onMove.Invoke(ctx.ReadValue<Vector2>());
			else if (ctx.canceled)
				onMove.Invoke(Vector2.zero);
		}

		public void OnUseInput(InputAction.CallbackContext ctx) {
			if (ctx.performed)
				onUse.Invoke();
			else if (ctx.canceled)
				onEndUse.Invoke();
		}

		public void OnPickupInput(InputAction.CallbackContext ctx) {
			if (ctx.performed)
				onPickup.Invoke();
		}

		public void OnDropInput(InputAction.CallbackContext ctx) {
			if (ctx.performed)
				onDrop.Invoke();
		}

		public void OnInteractInput(InputAction.CallbackContext ctx) {
			if (ctx.performed)
				onInteract.Invoke();
		}

		#endregion
	}
}