
namespace Audio {
	public enum SoundEffect {
		FoodReady,
		Gameplay_GuestEnter,
		Gameplay_RoundLost,
		Gameplay_RoundWon,
		Instrument_HarpPlay,
		Physics_CoinHit,
		Physics_TankardPlace,
		Player_PickUpCoin,
		Player_SwingAxe,
		PourBeer,
		Repair,
		Viking_AxeHit,
		Viking_DesireFilledMan
	}

	public static partial class AudioIndex {
		private static string[] soundEffectPaths = {
			"Audio/SoundEffects/FoodReady",
			"Audio/SoundEffects/Gameplay/GuestEnter",
			"Audio/SoundEffects/Gameplay/RoundLost",
			"Audio/SoundEffects/Gameplay/RoundWon",
			"Audio/SoundEffects/Instrument/HarpPlay",
			"Audio/SoundEffects/Physics/CoinHit",
			"Audio/SoundEffects/Physics/TankardPlace",
			"Audio/SoundEffects/Player/PickUpCoin",
			"Audio/SoundEffects/Player/SwingAxe",
			"Audio/SoundEffects/PourBeer",
			"Audio/SoundEffects/Repair",
			"Audio/SoundEffects/Viking/AxeHit",
			"Audio/SoundEffects/Viking/DesireFilledMan"
		};
	}
}