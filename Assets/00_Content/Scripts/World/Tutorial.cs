using System.Collections;
using Cameras;
using Interactables;
using Interactables.Beers;
using Interactables.Instruments;
using Interactables.Kitchens;
using Interactables.Weapons;
using Player;
using Scenes;
using Taverns;
using UI;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using Vikings;
using Vikings.States;

namespace World {
	public class Tutorial : MonoBehaviour {
		[Header("Tutorial objects")]
		[SerializeField] private PlayerComponent player;
		[SerializeField] private Tankard tankard;
		[SerializeField] private BeerTap beerTap;
		[SerializeField] private BeerCellar beerCellar;
		[SerializeField] private Table table;
		[SerializeField] private Instrument instrument;
		[SerializeField] private RepairTool tool;
		[SerializeField] private Axe weapon;
		[SerializeField] private Kitchen kitchen;

		[Space]
		[SerializeField] private DialogueText dialogueText;
		[SerializeField] private GameObject moveControls;
		[SerializeField] private GameObject interactControls;
		[SerializeField] private GameObject dropControls;
		[SerializeField] private GameObject continuePrompt;
		[SerializeField] private GameObject highlightPrefab;

		[Space]
		[SerializeField] private InputActionProperty continueAction;

		[Space]
		[SerializeField] private TutorialPhase[] phases;

		private int currentPhaseIndex;
		private TutorialPhase CurrentPhase => phases[currentPhaseIndex];
		private GameObject highlight;
		private bool highlightActive;
		private FollowingCamera cam;
		private Viking viking;

		private bool autoFill = true;
		private NavMeshAgent goblinAgent;
		private float goblinSpeed;

		private void Start() {
			if (PlayerManager.Instance != null)
				PlayerManager.Instance.AddPlayer(player);

			cam = Camera.main.GetComponent<FollowingCamera>();

			highlight = Instantiate(highlightPrefab);

			AddEventListeners();

			PreparePhase();
		}

		private void OnEnable() {
			continueAction.action.Enable();
			continueAction.action.performed += OnContinuePerformed;
		}

		private void AddEventListeners() {
			VikingController.Instance.VikingSpawned += vik => {
				viking = vik;
				viking.LeaveQueue += OnVikingLeaveQueue;
				viking.TakingSeat += OnVikingTakeSeat;
				viking.BecameSatisfied += OnVikingSatisfied;
				viking.SatisfiedEnd += OnVikingSatisfiedEnd;
				viking.Hit += OnVikingHit;
				viking.OrderTaken += OnVikingOrderTaken;
			};

			tankard.OnPickedUp += OnTankardPickedUp;
			tankard.OnSpilled += OnTankardSpilled;

			beerTap.BeerPoured += OnBeerPoured;
			beerTap.TapRefilled += OnBeerTapRefilled;

			beerCellar.beerBarrelSpawn += barrel => barrel.OnPickedUp += OnBeerBarrelPickedUp;

			kitchen.CookingFinished += OnCookingFinished;

			GoblinController.Instance.GoblinSpawned += OnGoblinSpawned;

			table.Destroyed += OnTableDestroyed;
			table.Repaired += OnTableRepaired;

			instrument.OnPickedUp += OnInstrumentPickedUp;
			instrument.OnDropped += OnInstrumentDropped;
			weapon.OnPickedUp += OnWeaponPickedUp;
			tool.OnPickedUp += OnToolPickedUp;

			Tavern.Instance.OnMoneyChanges += OnMoneyChanged;
		}

		private void OnGoblinSpawned(Goblin goblin) {
			goblinAgent = goblin.GetComponent<NavMeshAgent>();
			goblinSpeed = goblinAgent.speed;
			goblinAgent.speed = 0.01f;

			goblin.OnLeave += OnGoblinLeave;
		}

		private void OnTankardSpilled() {
			if (autoFill)
				beerTap.Refill();
		}

		private void OnDestroy() {
			if (PlayerManager.Instance != null)
				PlayerManager.Instance.RemovePlayer(player);
		}

		#region EventConversions
		private void OnVikingLeaveQueue(Viking sender) => OnTutorialEvent(TutorialEvent.VikingLeaveQueue);
		private void OnVikingTakeSeat() => OnTutorialEvent(TutorialEvent.VikingSeated);
		private void OnVikingSatisfied() => OnTutorialEvent(TutorialEvent.VikingSatisfied);
		private void OnVikingSatisfiedEnd() => OnTutorialEvent(TutorialEvent.VikingSatisfiedEnd);
		private void OnBeerPoured() => OnTutorialEvent(TutorialEvent.BeerPoured);
		private void OnBeerTapRefilled() => OnTutorialEvent(TutorialEvent.BeerTapRefilled);
		private void OnMoneyChanged() => OnTutorialEvent(TutorialEvent.MoneyEarned);
		private void OnTableDestroyed() => OnTutorialEvent(TutorialEvent.TableDestroyed);
		private void OnVikingHit() => OnTutorialEvent(TutorialEvent.VikingHit);
		private void OnTableRepaired() => OnTutorialEvent(TutorialEvent.TableRepaired);
		private void OnTankardPickedUp(PickUp _, PlayerComponent __) => OnTutorialEvent(TutorialEvent.TankardPickedUp);
		private void OnBeerBarrelPickedUp(PickUp _, PlayerComponent __) => OnTutorialEvent(TutorialEvent.BeerBarrelPickedUp);
		private void OnInstrumentPickedUp(PickUp _, PlayerComponent playerComponent) => OnTutorialEvent(TutorialEvent.InstrumentPickedUp);
		private void OnInstrumentDropped(PickUp _, PlayerComponent __) => OnTutorialEvent(TutorialEvent.InstrumentDropped);
		private void OnWeaponPickedUp(PickUp _, PlayerComponent playerComponent) => OnTutorialEvent(TutorialEvent.WeaponPickedUp);
		private void OnToolPickedUp(PickUp _, PlayerComponent playerComponent) => OnTutorialEvent(TutorialEvent.RepairToolPickedUp);
		private void OnVikingOrderTaken() => OnTutorialEvent(TutorialEvent.OrderTaken);
		private void OnCookingFinished(Food food) {
			food.OnPickedUp += OnFoodPickedUp;
			OnTutorialEvent(TutorialEvent.CookingFinished);
		}
		private void OnFoodPickedUp(PickUp _, PlayerComponent __) => OnTutorialEvent(TutorialEvent.FoodPickedUp);
		private void OnGoblinLeave(Goblin obj) => OnTutorialEvent(TutorialEvent.GoblinLeave);
		private void OnContinuePerformed(InputAction.CallbackContext obj) => OnTutorialEvent(TutorialEvent.ContinuePressed);

		#endregion

		private void OnTutorialEvent(TutorialEvent e) {
			if (currentPhaseIndex >= phases.Length) return;

			if (!CurrentPhase.SetDuration && CurrentPhase.IsCorrectEvent(e)) {
				if (!TryGoToNextPhase())
					return;

				PreparePhase();
			}
		}

		private bool TryGoToNextPhase() {
			currentPhaseIndex++;

			if (currentPhaseIndex >= phases.Length) {
				SceneLoadManager.Instance.LoadMainMenu();
				return false;
			}

			return true;
		}

		private void PreparePhase() {
			CurrentPhase.Enter(highlight);

			if (highlight.activeInHierarchy && !highlightActive)
				cam.AddTarget(highlight.transform);
			else if (!highlight.activeInHierarchy && highlightActive)
				cam.RemoveTarget(highlight.transform);

			highlightActive = highlight.activeInHierarchy;

			string trimmedText = CurrentPhase.Text.Trim();
			if (trimmedText.Length > 0)
				dialogueText.TypeText(trimmedText);

			if (CurrentPhase.SetDuration)
				StartCoroutine(CoWaitPhaseDuration(CurrentPhase.Duration));

			moveControls.SetActive(CurrentPhase.ShowMoveControls);
			interactControls.SetActive(CurrentPhase.ShowInteractControls);
			dropControls.SetActive(CurrentPhase.ShowDropControls);

			continuePrompt.SetActive(CurrentPhase.CompleteOn == TutorialEvent.ContinuePressed);
		}

		private IEnumerator CoWaitPhaseDuration(float duration) {
			yield return new WaitForSeconds(duration);

			if (TryGoToNextPhase())
				PreparePhase();
		}

		#region Unity Event Handlers

		public void StartBrawl() => viking.ForceChangeState(new BrawlingVikingState(viking, table));
		public void DisableAutoFill() => autoFill = false;
		public void SpawnGoblin() => GoblinController.Instance.MaxGoblins = 1;
		public void DisableGoblinSpawning() => GoblinController.Instance.MaxGoblins = 0;
		public void EnableGoblin() => goblinAgent.speed = goblinSpeed;
		public void NullifyViking() => viking.ForceChangeState(new NullVikingState(viking));

		#endregion
	}
}
