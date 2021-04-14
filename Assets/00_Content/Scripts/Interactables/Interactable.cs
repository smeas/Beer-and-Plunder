using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Interactables {
	public class Interactable : MonoBehaviour {

		public virtual void CancelInteraction() {
		}
		public virtual void Interact() {
		}
	}
}
