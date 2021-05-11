
using System.Collections.Generic;

namespace Audio {
	public enum SoundEffect {
		Gameplay_GuestEnter = -135389647,
		Gameplay_RoundLost = 780448425,
		Gameplay_RoundWon = 1687523630,
		Instrument_HarpPlay = -351740733,
		Physics_CoinHit = 1668061808,
		Physics_SpillBeer = -917762040,
		Physics_TankardPlace = -1172795835,
		Player_PickUpCoin = 594944396,
		Player_SwingAxe = 1556566438,
		PourBeer = 387602004,
		Repair = -39618835,
		Viking_AxeHit = 382397011,
		Viking_DesireFilledMan = -574487926
	}

	public static partial class AudioIndex {
		private static Dictionary<int, string> soundEffectPaths = new Dictionary<int, string> {
			{ -135389647, "Audio/SoundEffects/Gameplay/GuestEnter" },
			{ 780448425, "Audio/SoundEffects/Gameplay/RoundLost" },
			{ 1687523630, "Audio/SoundEffects/Gameplay/RoundWon" },
			{ -351740733, "Audio/SoundEffects/Instrument/HarpPlay" },
			{ 1668061808, "Audio/SoundEffects/Physics/CoinHit" },
			{ -917762040, "Audio/SoundEffects/Physics/SpillBeer" },
			{ -1172795835, "Audio/SoundEffects/Physics/TankardPlace" },
			{ 594944396, "Audio/SoundEffects/Player/PickUpCoin" },
			{ 1556566438, "Audio/SoundEffects/Player/SwingAxe" },
			{ 387602004, "Audio/SoundEffects/PourBeer" },
			{ -39618835, "Audio/SoundEffects/Repair" },
			{ 382397011, "Audio/SoundEffects/Viking/AxeHit" },
			{ -574487926, "Audio/SoundEffects/Viking/DesireFilledMan" }
		};
	}
}