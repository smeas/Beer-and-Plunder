using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Interactables {

	[CreateAssetMenu(fileName = "Beer", menuName = "Beer")]
	public class BeerData : ScriptableObject {
		public BeerType type;
		public int cost;
	}
}
