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
	[CustomEditor(typeof(DesireData))]
	public class DesireEditor : Editor {
		public override void OnInspectorGUI() {

			base.OnInspectorGUI();

			var desireDataScript = target as DesireData;
				desireDataScript.isOrder = EditorGUILayout.Toggle("IsOrder", desireDataScript.isOrder);

			if (desireDataScript.isOrder) // if bool is true, show other fields
			{
				desireDataScript.visualisationAfterPrefab = EditorGUILayout.ObjectField("visualisationAfterPrefab", desireDataScript.visualisationAfterPrefab, typeof(GameObject), true) as GameObject;
			}
		}
	}
}
