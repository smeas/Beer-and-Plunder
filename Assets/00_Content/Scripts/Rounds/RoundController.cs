using System;
using System.Collections;
using Audio;
using Cameras;
using Cinemachine;
using Interactables;
using Player;
using Taverns;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using Utilities;
using Vikings;

namespace Rounds {
	[DefaultExecutionOrder(-1)]
	public class RoundController : SingletonBehaviour<RoundController> {
		[SerializeField] private ScalingData[] playerDifficulties;
		[SerializeField] private ScoreCard scoreCardPrefab;
		[SerializeField] private GameOver gameOverPanelPrefab;
		[SerializeField, Tooltip("seconds/round")]
		private int roundDuration;
		[SerializeField] private int requiredMoney = 250;

		[Header("Timeline")]
		[SerializeField] private PlayableDirector timelineDirector;
		[SerializeField] private PlayableAsset showScoreCardTimeline;
		[SerializeField] private PlayableAsset hideScoreCardTimeline;
		[SerializeField] private CinemachineVirtualCamera virtualFollowingCamera;

		private GameOver gameOverPanel;
		private FollowingCamera followingCamera;
		private ScoreCard scoreCard;
		private SoundHandle clockTickSound;
		private float roundTimer;
		private int currentRound = 1;
		private bool isRoundActive = true;
		private bool isGamePlayActive = true;
		private bool isTenSecondTimerStarted = false;
		//ensures the horn has only sounded once at end of round

		public ScalingData CurrentDifficulty =>
			playerDifficulties[PlayerManager.Instance && PlayerManager.Instance.NumPlayers > 0
			? PlayerManager.Instance.NumPlayers - 1
			: 0];

		public event Action OnRoundOver;
		public event Action OnNewRoundStart;

		public int RoundDuration => roundDuration;
		public float RoundTimer => roundTimer;
		public int RequiredMoney => requiredMoney;
		public bool IsRoundActive => isRoundActive;
		public bool IsGamePlayActive => isGamePlayActive;

		private void Start() {
			scoreCard = Instantiate(scoreCardPrefab);
			scoreCard.gameObject.SetActive(false);
			gameOverPanel = Instantiate(gameOverPanelPrefab);
			gameOverPanel.gameObject.SetActive(false);

			scoreCard.OnNextRound += HandleOnNextRound;
			Table.OnTablesDestroyed += HandleOnTablesDestroyed;

			followingCamera = Camera.main.GetComponent<FollowingCamera>();

			SendNextDifficulty();
		}

		private void OnDisable() {
			clockTickSound?.Stop();
		}

		private void Update() {
			if (!isGamePlayActive) return;

			roundTimer += Time.deltaTime;

			if (roundDuration - roundTimer <= 10 && !isTenSecondTimerStarted) {
				clockTickSound = AudioManager.Instance.PlayEffect(SoundEffect.ClockTick, true);
				isTenSecondTimerStarted = true;
			}

			if (roundTimer >= roundDuration) {
				clockTickSound.Stop();
				isTenSecondTimerStarted = false;
				RoundOver();
				isGamePlayActive = false;
			}
		}

		private void RoundOver() { 
			AudioManager.Instance.PlayEffect(SoundEffect.Gameplay_WarHorn);

			StartCoroutine(CoWaitForVikingsLeaving());
		}

		private IEnumerator CoWaitForVikingsLeaving() {
			if (VikingController.Instance != null) {
				VikingController.Instance.CanSpawn = false;
				VikingController.Instance.LeaveAllVikings();

				while (VikingController.Instance.VikingCount > 0)
					yield return null;
			}

			DisableGamePlay();
			isRoundActive = false;
			OnRoundOver?.Invoke();

			if (Tavern.Instance != null && Tavern.Instance.Money < requiredMoney) {
				TavernBankrupt();
				Debug.Log($"Required money goal was not reached. ({Tavern.Instance.Money}/{requiredMoney})");
			}
			else {
				ShowScoreCard();
			}
		}
		
		private void SendNextDifficulty() {
			if (VikingController.Instance == null) return;

			ScalingData difficulty = CurrentDifficulty;

			VikingController.Instance.SetSpawnSettings(difficulty.ScaledSpawnDelay(currentRound), difficulty.spawnDelayVariance);
			VikingController.Instance.StatScaling = new VikingScaling(difficulty, currentRound);
		}

		private void DisableGamePlay() {
			if (VikingController.Instance != null) VikingController.Instance.CanSpawn = false;

			if (PlayerManager.Instance == null) {
				Debug.Assert(false, "RoundController can't find a PlayerManager instance.");
				return;
			}

			foreach (PlayerComponent player in PlayerManager.Instance.Players) {
				PlayerInput playerInput = player.GetComponent<PlayerInput>();
				playerInput.SwitchCurrentActionMap("UI");
			}
		}

		private void ShowScoreCard() {

			Transform camTransform = followingCamera.transform;
			virtualFollowingCamera.transform.SetPositionAndRotation(camTransform.position, camTransform.rotation);

			scoreCard.UpdateScoreCard(currentRound);

			AudioManager.PlayEffectSafe(SoundEffect.Gameplay_RoundWon);

			StartCoroutine(CoMoveToScoreCard());
		}

		private IEnumerator CoMoveToScoreCard() {
			timelineDirector.playableAsset = showScoreCardTimeline;
			timelineDirector.Play();

			yield return new WaitForSeconds((float)showScoreCardTimeline.duration);

			scoreCard.Show();
		}

		private void EnableGamePlay() {
			if (VikingController.Instance != null) VikingController.Instance.CanSpawn = true;

			if (PlayerManager.Instance == null) {
				Debug.Assert(false, "RoundController can't find a PlayerManager instance.");
				return;
			}

			foreach (PlayerComponent player in PlayerManager.Instance.Players) {
				PlayerInput playerInput = player.GetComponent<PlayerInput>();
				playerInput.SwitchCurrentActionMap("Game");
			}
		}

		private void HandleOnNextRound() {
			isRoundActive = true;
			isGamePlayActive = true;
			currentRound++;
			SendNextDifficulty();
			roundTimer = 0f;

			if (Tavern.Instance != null)
				Tavern.Instance.Money = Tavern.Instance.StartingMoney;

			OnNewRoundStart?.Invoke();

			StartCoroutine(CoLeaveScoreCard());
		}

		private IEnumerator CoLeaveScoreCard() {
			followingCamera.transform.rotation = virtualFollowingCamera.transform.rotation;

			Vector3 desiredPosition = followingCamera.CalculateTargetPosition();

			if (followingCamera.UseBounds)
				desiredPosition = followingCamera.ConstrainPositionInBounds(desiredPosition);

			virtualFollowingCamera.transform.position = desiredPosition;

			timelineDirector.playableAsset = hideScoreCardTimeline;
			timelineDirector.Play();

			yield return new WaitForSeconds((float)hideScoreCardTimeline.duration);

			EnableGamePlay();
		}

		private void HandleOnTablesDestroyed() {
			isRoundActive = false;
			OnRoundOver?.Invoke();
			DisableGamePlay();
			gameOverPanel.Show(LoseCondition.Destruction);
			AudioManager.PlayEffectSafe(SoundEffect.Gameplay_RoundLost);
		}

		private void TavernBankrupt() {
			isRoundActive = false;
			gameOverPanel.Show(LoseCondition.Bankrupcy);
			AudioManager.PlayEffectSafe(SoundEffect.Gameplay_RoundLost);
		}
	}
}
