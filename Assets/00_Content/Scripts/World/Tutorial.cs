using Cameras;
using Interactables;
using Interactables.Beers;
using Interactables.Instruments;
using Interactables.Weapons;
using Player;
using Scenes;
using Taverns;
using UI;
using UnityEngine;
using Vikings;
using Vikings.States;

namespace World {
	public class Tutorial : MonoBehaviour {
		[Header("Tutorial objects")]
		[SerializeField] private PlayerComponent player;
		[SerializeField] private Viking viking;
		[SerializeField] private BeerTap beerTap;
		[SerializeField] private Table table;
		[SerializeField] private Instrument instrument;
		[SerializeField] private RepairTool tool;
		[SerializeField] private Axe weapon;

		[Space]
		[SerializeField] private DialogueText dialogueText;
		[SerializeField] private GameObject highlightPrefab;

		[Space]
		[SerializeField] private TutorialPhase[] phases;

		private int currentPhaseIndex;
		private TutorialPhase CurrentPhase => phases[currentPhaseIndex];
		private GameObject highlight;
		private bool highlightActive;
		private FollowingCamera cam;

		private void Start() {
			if (PlayerManager.Instance != null)
				PlayerManager.Instance.AddPlayer(player);

			cam = Camera.main.GetComponent<FollowingCamera>();

			highlight = Instantiate(highlightPrefab);

			AddEventListeners();

			PreparePhase();
		}

		private void AddEventListeners() {
			viking.LeaveQueue += OnVikingLeaveQueue;
			viking.TakingSeat += OnVikingTakeSeat;
			viking.BecameSatisfied += OnVikingSatisfied;
			viking.Hit += OnVikingHit;

			beerTap.BeerPoured += OnBeerPoured;
			beerTap.TapRefilled += OnBeerTapRefilled;

			table.Destroyed += OnTableDestroyed;
			table.Repaired += OnTableRepaired;

			instrument.OnPickedUp += OnInstrumentPickedUp;
			weapon.OnPickedUp += OnWeaponPickedUp;
			tool.OnPickedUp += OnToolPickedUp;

			Tavern.Instance.OnMoneyChanges += OnMoneyChanged;
		}

		#region EventConversions

		private void OnVikingLeaveQueue(Viking sender) => OnTutorialEvent(TutorialEvent.VikingLeaveQueue);
		private void OnVikingTakeSeat() => OnTutorialEvent(TutorialEvent.VikingSeated);
		private void OnVikingSatisfied() => OnTutorialEvent(TutorialEvent.VikingSatisfied);
		private void OnBeerPoured() => OnTutorialEvent(TutorialEvent.BeerPoured);
		private void OnBeerTapRefilled() => OnTutorialEvent(TutorialEvent.BeerTapRefilled);
		private void OnMoneyChanged() => OnTutorialEvent(TutorialEvent.MoneyEarned);
		private void OnTableDestroyed() => OnTutorialEvent(TutorialEvent.TableDestroyed);
		private void OnVikingHit() => OnTutorialEvent(TutorialEvent.VikingHit);
		private void OnTableRepaired() => OnTutorialEvent(TutorialEvent.TableRepaired);
		private void OnInstrumentPickedUp(PickUp _) => OnTutorialEvent(TutorialEvent.InstrumentPickedUp);
		private void OnWeaponPickedUp(PickUp _) => OnTutorialEvent(TutorialEvent.WeaponPickedUp);
		private void OnToolPickedUp(PickUp _) => OnTutorialEvent(TutorialEvent.RepairToolPickedUp);

		#endregion

		private void OnTutorialEvent(TutorialEvent e) {
			if (currentPhaseIndex >= phases.Length) return;

			if (CurrentPhase.OnEvent(e)) {
				currentPhaseIndex++;

				if (currentPhaseIndex >= phases.Length) {
					if (PlayerManager.Instance != null)
						PlayerManager.Instance.RemovePlayer(player);

					SceneLoadManager.Instance.LoadMainMenu();
					return;
				}

				PreparePhase();
			}
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
		}

		#region EventFunctions

		public void StartBrawl() {
			viking.ForceChangeState(new BrawlingVikingState(viking, table));
		}

		#endregion
	}
}
