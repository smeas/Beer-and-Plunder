using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Player {
	public class PlayerInputHandler : MonoBehaviour {
		[SerializeField] private UnityEvent<Vector2> onMove;
		[SerializeField] private UnityEvent onDrop;
		[SerializeField] private UnityEvent onInteract;
		[SerializeField] private UnityEvent onEndInteract;
		[SerializeField] private UnityEvent onStart;
		[SerializeField] private UnityEvent onSubmit;

		public UnityEvent<Vector2> OnMove => onMove;
		public UnityEvent OnDrop => onDrop;
		public UnityEvent OnInteract => onInteract;
		public UnityEvent OnEndInteract => onEndInteract;
		public UnityEvent OnStart => onStart;
		public UnityEvent OnSubmit => onSubmit;

		private PlayerInput playerInput;

		// Workaround for a bug in the in the input system where events are executed on the prefab.
		private bool ShouldExecuteEvents {
			get {
				if (playerInput == null)
					playerInput = GetComponent<PlayerInput>();

				// Check if we are a real player
				return playerInput.playerIndex != -1;
			}
		}

		// These methods are invoked by the UnityEvents on the PlayerInput component.
		#region Input Action Handlers

		// Game

		public void OnMoveInput(InputAction.CallbackContext ctx) {
			if (!ShouldExecuteEvents) return;
			if (ctx.performed)
				onMove.Invoke(ctx.ReadValue<Vector2>());
			else if (ctx.canceled)
				onMove.Invoke(Vector2.zero);
		}

		public void OnDropInput(InputAction.CallbackContext ctx) {
			if (!ShouldExecuteEvents) return;
			if (ctx.performed)
				onDrop.Invoke();
		}

		public void OnInteractInput(InputAction.CallbackContext ctx) {
			if (!ShouldExecuteEvents) return;
			if (ctx.performed)
				onInteract.Invoke();
			else if (ctx.canceled)
				onEndInteract.Invoke();
		}

		// UI

		public void OnStartInput(InputAction.CallbackContext ctx) {
			if (!ShouldExecuteEvents) return;
			if (ctx.performed)
				onStart.Invoke();
		}

		public void OnSubmitInput(InputAction.CallbackContext ctx) {
			if (!ShouldExecuteEvents) return;
			if (ctx.performed)
				onSubmit.Invoke();
		}

		#endregion
	}
}
