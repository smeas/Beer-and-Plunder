using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactables {
	public interface IInteractable {

		public void Interact();

		public void CancelInteraction();
	}
}
