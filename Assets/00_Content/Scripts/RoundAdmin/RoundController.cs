using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Taverns;

namespace Rounds {
	public class RoundController : MonoBehaviour {
		[SerializeField] private Tavern tavern;
		[SerializeField] private GameObject vikingManager;

		//variables for time and for sending waves.
		[SerializeField] private int amountOfWaves;
		[SerializeField] private int timeBetweenWave;

		//variables for the waves, will switch this to objects and list longer on probably
		private float timeSinceWave;
		private int[] Waves;
		//private int indexOfWave = 0;

		private void Start() {

			tavern.Bankrupt += OnTavernBankrupt;

			tavern.Destroyed += TavernDestroyed;

			SetUpWaves();

		}

		private void Update() {

			CheckWave();
		}

		private void SetUpWaves() {

			Waves = new int[amountOfWaves];

			for (int i = 0; i < Waves.Length; i++) {
				Waves[i] = timeBetweenWave;
			}
		}

		private void CheckWave() {
			timeSinceWave += Time.deltaTime;

			if (timeSinceWave > timeBetweenWave) {

				//send the object on the list to VikingManager here.

				timeSinceWave = 0;
			}
		}

		private void SendNextWave() { //Sends to vikingManager
		}

		private void TavernDestroyed() {
			throw new System.NotImplementedException();
		}

		private void OnTavernBankrupt() {
			throw new System.NotImplementedException();
		}
	}
}
