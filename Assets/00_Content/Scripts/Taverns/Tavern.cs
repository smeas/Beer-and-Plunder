using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Taverns {
	public class Tavern : MonoBehaviour {

		public event Action Bankrupt;
		public event Action Destroyed;

		private bool IsBankrupt => Money <= 0;
		private bool IsDestroyed => Health <= 0;

		[SerializeField] private int maxSittingGuests;

		private int sittingGuests;
		public int SittingGuests {
			get => sittingGuests;
			set { sittingGuests = Mathf.Clamp(value, 0, maxSittingGuests); }
		}

		[SerializeField] private float maxHealth = 100;
		[SerializeField] private float startingHealth = 100;

		private float health;
		public float Health {
			get => health;
			set { health = Mathf.Round(Mathf.Clamp(value, -1, maxHealth)); }
		}
		
		[SerializeField] private float startingMoney = 50;
		[SerializeField] private float maxMoney = 100;

		private float money;
		public float Money {
			get => money;
			set => money = Mathf.Round(Mathf.Clamp(value, -1, maxMoney));
		}

		void Start() {
			Health = startingHealth;
			Money = startingMoney;
			SittingGuests = 0;
		}

		public void EarnsMoney(float moneyEarned) {   
			Money += moneyEarned;
		}

		public void SpendsMoney(float moneySpent) {	Money -= moneySpent;
			if(IsBankrupt)
			{ Debug.Log("game over, you are bankrupt!");

				Bankrupt?.Invoke();
			}
		}

		public void TakesDamage(float damage) {	
			Health -= damage;

			if (IsDestroyed) {
				Debug.Log("game over, your tavern is destroyed!");

				Destroyed?.Invoke();
			}
		}

		public void RepairsDamage(float repair) {	
			Health += repair;
		}

		public void GuestSitsDown() {
			SittingGuests++;
		}

		public void GuestStandsUp() {
			SittingGuests--;
		}
	}
}