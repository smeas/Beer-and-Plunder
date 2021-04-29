using System;
using System.Collections.Generic;
using System.Linq;
using Rounds;
using UnityEngine;

namespace Vikings {
	public class VikingStats {
		private List<StatModifier> modifiers = new List<StatModifier>();

		private float moodDeclineRate;
		private float moodDeclineModifier = 1f;
		private float brawlHealth;

		public float Mood { get; private set; }
		public float StartMood { get; private set; }
		public float BrawlHealth => brawlHealth;

		public VikingStats(VikingData data, VikingScaling scaling) {
			StartMood = data.startMood * scaling.startMoodMultiplier;
			Mood = StartMood;
			brawlHealth = data.brawlHealth;
			moodDeclineRate = data.moodDeclineRate * scaling.moodDeclineMultiplier;
		}

		public void Decline() {
			Mood -= moodDeclineRate * moodDeclineModifier * Time.deltaTime;
		}

		public void AddModifier(StatModifier modifier) {
			modifiers.Add(modifier);
			RecalculateModifier(modifier.statType);
		}

		public void RemoveModifier(StatModifier modifier) {
			bool success = modifiers.Remove(modifier);

			Debug.Assert(success, "Successfully removed modifier");
			RecalculateModifier(modifier.statType);
		}

		public void TakeMoodDamage(float moodDamage) {
			Mood -= moodDamage;
		}

		public void TakeBrawlDamage(float brawlDamage) {
			brawlHealth -= brawlDamage;
		}

		private void RecalculateModifier(StatType type) {
			switch (type) {
				case StatType.MoodDeclineRate:
					moodDeclineModifier = 1 + modifiers
						.Where(x => x.statType == StatType.MoodDeclineRate)
						.Sum(x => x.value);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public void Reset() {
			Mood = StartMood;
		}
	}
}
