
using System.Collections.Generic;

namespace Audio {
	public enum SoundEffect {
		ClockTick = 385307467,
		Cooking = -195161644,
		FoodReady = 1745492459,
		Gameplay_GuestEnter = -135389647,
		Gameplay_RoundLost = 780448425,
		Gameplay_RoundWon = 1687523630,
		Gameplay_WarHorn = -950789667,
		Goblin_GoblinHit = 118386714,
		Goblin_GoblinLaugh = -1546456542,
		Instrument_HarpPlay = -351740733,
		Misc_Poof = -1764842801,
		Physics_AxeDrop = -566482055,
		Physics_CoinHit = 1668061808,
		Physics_HammerDrop = -1142848681,
		Physics_HarpDrop = -753439835,
		Physics_SpillBeer = -917762040,
		Physics_TankardDrop = -639203100,
		Physics_TankardPlace = -1172795835,
		Player_HammerRepairHit = 2049981692,
		Player_PickUpCoin = 594944396,
		Player_SwingAxe = 1556566438,
		PourBeer = 387602004,
		Viking_AxeHit = 382397011,
		Viking_Brawling_Angry = 1516207403,
		Viking_Brawling_TableHit = -1066163748,
		Viking_Desire_DesireFullfilled = -799215494,
		Viking_Desire_Irritated = -98052361,
		Viking_Desire_NeedBeer = 54628094,
		Viking_Desire_NeedFood = -272874605,
		Viking_Desire_NeedMusic = -831503651,
		Viking_Desire_NeedTable = 1309949203
	}

	public static partial class AudioIndex {
		private static Dictionary<int, string> soundEffectPaths = new Dictionary<int, string> {
			{ 385307467, "Audio/SoundEffects/ClockTick" },
			{ -195161644, "Audio/SoundEffects/Cooking" },
			{ 1745492459, "Audio/SoundEffects/FoodReady" },
			{ -135389647, "Audio/SoundEffects/Gameplay/GuestEnter" },
			{ 780448425, "Audio/SoundEffects/Gameplay/RoundLost" },
			{ 1687523630, "Audio/SoundEffects/Gameplay/RoundWon" },
			{ -950789667, "Audio/SoundEffects/Gameplay/WarHorn" },
			{ 118386714, "Audio/SoundEffects/Goblin/GoblinHit" },
			{ -1546456542, "Audio/SoundEffects/Goblin/GoblinLaugh" },
			{ -351740733, "Audio/SoundEffects/Instrument/HarpPlay" },
			{ -1764842801, "Audio/SoundEffects/Misc/Poof" },
			{ -566482055, "Audio/SoundEffects/Physics/AxeDrop" },
			{ 1668061808, "Audio/SoundEffects/Physics/CoinHit" },
			{ -1142848681, "Audio/SoundEffects/Physics/HammerDrop" },
			{ -753439835, "Audio/SoundEffects/Physics/HarpDrop" },
			{ -917762040, "Audio/SoundEffects/Physics/SpillBeer" },
			{ -639203100, "Audio/SoundEffects/Physics/TankardDrop" },
			{ -1172795835, "Audio/SoundEffects/Physics/TankardPlace" },
			{ 2049981692, "Audio/SoundEffects/Player/HammerRepairHit" },
			{ 594944396, "Audio/SoundEffects/Player/PickUpCoin" },
			{ 1556566438, "Audio/SoundEffects/Player/SwingAxe" },
			{ 387602004, "Audio/SoundEffects/PourBeer" },
			{ 382397011, "Audio/SoundEffects/Viking/AxeHit" },
			{ 1516207403, "Audio/SoundEffects/Viking/Brawling/Angry" },
			{ -1066163748, "Audio/SoundEffects/Viking/Brawling/TableHit" },
			{ -799215494, "Audio/SoundEffects/Viking/Desire/DesireFullfilled" },
			{ -98052361, "Audio/SoundEffects/Viking/Desire/Irritated" },
			{ 54628094, "Audio/SoundEffects/Viking/Desire/NeedBeer" },
			{ -272874605, "Audio/SoundEffects/Viking/Desire/NeedFood" },
			{ -831503651, "Audio/SoundEffects/Viking/Desire/NeedMusic" },
			{ 1309949203, "Audio/SoundEffects/Viking/Desire/NeedTable" }
		};
	}
}