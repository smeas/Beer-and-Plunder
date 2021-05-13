using Interactables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Vikings;

namespace Interactables.Kitchens {
	public class Food : PickUp, IDesirable {

		[SerializeField] private FoodData foodData;

		public DesireType DesireType => foodData.type;
	}
}
