using System;
using UnityEngine;
using Utilities;

namespace Taverns {
	public class Tavern : SingletonBehaviour<Tavern> {

		[SerializeField] private int maxSittingGuests;
		[SerializeField] private float maxHealth = 100;
		[SerializeField] private float startingHealth = 100;
		[SerializeField] private int startingMoney = 0;

		private bool IsDestroyed => Health <= 0;

		private float health;
		private int money;

		public event Action OnDestroyed;
		public event Action OnMoneyChanges;
		public event Action OnHealthChanges;

		public float MaxHealth => maxHealth;
		public float StartingHealth => startingHealth;
		public int StartingMoney => startingMoney;

		public float Health {
			get => health;
			set { health = Mathf.Round(Mathf.Clamp(value, 0, maxHealth)); }
		}

		public int Money {
			get => money;
			set {
				int newMoney = money;
				
				if (money == newMoney)
					return;

				if (newMoney < 0)
					newMoney = 0;

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
			Money += Mathf.RoundToInt(moneyEarned);
		}

		public void TakesDamage(float damage) {
			if (IsDestroyed) return;

			Health -= damage;
			OnHealthChanges?.Invoke();

			if (IsDestroyed) {
				Debug.Log("Tavern says: Game over, your tavern is destroyed!");

				OnDestroyed?.Invoke();
			}
		}

		public void RepairsDamage(float repair) {
			Health += repair;
			OnHealthChanges?.Invoke();
		}
	}
}