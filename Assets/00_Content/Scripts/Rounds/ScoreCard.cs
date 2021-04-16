using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreCard : MonoBehaviour {
	[SerializeField] private GameObject scoreCardUI;

	private void Awake() {
		scoreCardUI.SetActive(false);
	}

	private void Update() {
		//TODO: Get number of active players,
		//TODO: Enable that many icons on UI
		//TODO: Wait for input from each player
		//TODO: Have their icon switch over when they give correct input
		//TODO: Have the icon switch back if player gives same input again.
		//TODO: When all have given input start next round.
	}

	public void OnEnable() {
		Time.timeScale = 0f;
	}

	public void StartNextRound() {
		Time.timeScale = 1f;

		scoreCardUI.SetActive(false);
	}
}
