using UnityEngine;

namespace Utilities {
	[CreateAssetMenu(fileName = "new DefaultSettings", menuName = "Game/DefaultSettings", order = 0)]
	public class DefaultSettingsData : ScriptableObject {
		[SerializeField] public float masterVolume = 0.5f;
		[SerializeField] public float musicVolume = 0.5f;
		[SerializeField] public float sfxVolume = 0.5f;
	}
}
