using Interactables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Vikings;

namespace Editors {
	[CustomEditor(typeof(Table))]
	public class TableEditor : Editor {
		public override void OnInspectorGUI() {
			base.OnInspectorGUI();

			Table table = target as Table;
			if (GUILayout.Button("Destroy Table")) {
				table.Damage(table.maxHealth);
			} 
		}
	}
}
