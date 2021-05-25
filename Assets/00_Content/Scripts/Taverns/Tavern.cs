using System;
using System.Collections;
using UnityEngine;
using Utilities;

namespace Taverns {
	public class Tavern : SingletonBehaviour<Tavern> {
		[SerializeField] private int startingMoney = 0;

		private int money;

		public event Action OnMoneyChanges;

		public int StartingMoney => startingMoney;

		public int Money {
			get => money;
			set {
				int newMoney = value;

				if (newMoney < 0)
					newMoney = 0;

				if (money == newMoney)
					return;

				money = newMoney;
				OnMoneyChanges?.Invoke();
			}
		}

		protected override void Awake() {
			base.Awake();
			Money = startingMoney;
		}

		public void EarnsMoney(int moneyEarned) {
			Money += moneyEarned;
		}

		public void ResetMoney(float duration) {
			StartCoroutine(CoResetMoney(duration));
		}

		private IEnumerator CoResetMoney(float duration) {
			if (money == 0) yield break;

			float moneyRemaining = money;
			int moneyAtStart = money;

			for (float time = 0; time < duration; time += Time.deltaTime) {
				moneyRemaining -= moneyAtStart / duration * Time.deltaTime;
				Money = Mathf.RoundToInt(moneyRemaining);
				yield return null;
			}
		}
	}
}
