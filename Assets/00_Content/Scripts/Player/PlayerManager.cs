using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;

namespace Player {
	[DefaultExecutionOrder(-10)]
	[RequireComponent(typeof(PlayerInputManager))]
	public class PlayerManager : SingletonBehaviour<PlayerManager> {
		// The player limit is enforced by the PlayerInputManager component which is configured through the inspector.
		public const int MaxPlayers = 4;

		private PlayerInputManager playerInputManager;
		private bool allowJoining;

		public bool AllowJoining {
			get => allowJoining;
			set {
				allowJoining = value;
				UpdateJoiningEnabled();
			}
		}

		public List<PlayerComponent> Players { get; } = new List<PlayerComponent>();

		public int NumPlayers => Players.Count;

		public event Action<PlayerComponent> PlayerJoined;

		/// <summary>
		/// This event is invoked when a player is disabled or destroyed.
		/// </summary>
		public event Action<PlayerComponent> PlayerLeft;

		protected override void Awake() {
			base.Awake();

			playerInputManager = GetComponent<PlayerInputManager>();
			playerInputManager.onPlayerJoined += OnPlayerJoined;
			playerInputManager.onPlayerLeft += OnPlayerLeft;

			DontDestroyOnLoad(this);
		}

		[CanBeNull]
		public PlayerComponent GetPlayerById(int id) {
			foreach (PlayerComponent player in Players) {
				if (player.PlayerId == id)
					return player;
			}

			return null;
		}

		public void AddPlayer(PlayerComponent player) {
			Players.Add(player);

			UpdateJoiningEnabled();
		}

		public void RemovePlayer(PlayerComponent player) {
			Players.Remove(player);

			UpdateJoiningEnabled();
		}

		private void UpdateJoiningEnabled() {
			bool shouldEnableJoining = NumPlayers < MaxPlayers && allowJoining;
			if (shouldEnableJoining == playerInputManager.joiningEnabled)
				return;

			if (shouldEnableJoining)
				playerInputManager.EnableJoining();
			else
				playerInputManager.DisableJoining();
		}

		#region Event Handlers

		private void OnPlayerJoined(PlayerInput playerInput) {
			UpdateJoiningEnabled();

			PlayerComponent player = playerInput.GetComponent<PlayerComponent>();
			if (player == null) {
				Debug.Assert(false, "Joined player has PlayerComponent");
				return;
			}

			player.Initialize();
			Players.Add(player);
			PlayerJoined?.Invoke(player);
		}

		private void OnPlayerLeft(PlayerInput playerInput) {
			UpdateJoiningEnabled();

			PlayerComponent player = playerInput.GetComponent<PlayerComponent>();
			if (player == null) {
				Debug.Assert(false, "Leaving player has PlayerComponent");
				return;
			}

			if (!Players.Remove(player))
				Debug.Assert(false, "Unknown player left");
			PlayerLeft?.Invoke(player);
		}

		#endregion
	}
}
