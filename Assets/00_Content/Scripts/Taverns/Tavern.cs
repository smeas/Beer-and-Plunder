using System;
using UnityEngine;
using Utilities;

namespace Taverns {
	public class Tavern : SingletonBehaviour<Tavern> {

		[SerializeField] private int maxSittingGuests;
		[SerializeField] private float maxHealth = 100;
		[SerializeField] private float startingHealth = 100;
		[SerializeField] private float startingMoney = 50;
		[SerializeField] private float maxMoney = 100;

		private bool IsBankrupt => Money <= 0;
		private bool IsDestroyed => Health <= 0;

		private float health;
		private float money;

		public event Action OnBankrupcy;
		public event Action OnDestroyed;
		public event Action OnMoneyChanges;
		public event Action OnHealthChanges;

		public float MaxHealth => maxHealth;
		public float StartingHealth => startingHealth;
		public float StartingMoney => startingMoney;
		public float MaxMoney => maxMoney;

		public float Health {
			get => health;
			set { health = Mathf.Round(Mathf.Clamp(value, 0, maxHealth)); }
		}

		public float Money {
			get => money;
			set {
				float newMoney = Mathf.Round(Mathf.Clamp(value, 0, maxMoney));
				if (money == newMoney)
					return;

				money = newMoney;
				OnMoneyChanges?.Invoke();
			}
		}

		protected override void Awake() {
			base.Awake();
			Health = startingHealth;
			Money = startingMoney;
		}

		public void EarnsMoney(float moneyEarned) {
			Money += moneyEarned;
		}

		public void SpendsMoney(float moneySpent) {
			Money -= moneySpent;

			if (IsBankrupt) {
				Debug.Log("game over, you are bankrupt!");

				OnBankrupcy?.Invoke();
			}
		}

		public void TakesDamage(float damage) {
			Health -= damage;
			OnHealthChanges?.Invoke();

			if (IsDestroyed) {
				Debug.Log("game over, your tavern is destroyed!");

				OnDestroyed?.Invoke();
			}
		}

		public void RepairsDamage(float repair) {
			Health += repair;
			OnHealthChanges?.Invoke();
		}
	}
}