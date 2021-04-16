using UnityEngine;
using Vikings;

namespace Player {
	public class PlayerSteward : MonoBehaviour {
		public Viking Follower { get; private set; }

		public void BeginSeatingViking(Viking viking) {
			if (Follower != null) {
				Debug.Assert(false, "Player does not have a follower");
				return;
			}

			Follower = viking;
		}

		public void EndSeatingViking(Viking viking) {
			if (viking != Follower) {
				Debug.Assert(false, "Viking is the follower");
				return;
			}

			Follower = null;
		}
	}
}