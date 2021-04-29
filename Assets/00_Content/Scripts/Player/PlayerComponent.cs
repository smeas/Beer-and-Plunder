using Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Vikings;

namespace Player {
	public class PlayerComponent : MonoBehaviour {

		[SerializeField] private PlayerData playerData;
		
		public int PlayerId { get; private set; }
		public PlayerData PlayerData { get => playerData; set => playerData = value; }

		private void Awake() {

			PlayerInput playerInput = GetComponent<PlayerInput>();
			PlayerId = playerInput.playerIndex;

			if (playerData == null) {
				Debug.LogWarning("PlayerData is not set in PlayerComponent");
				return;
			}
		}
	}
}