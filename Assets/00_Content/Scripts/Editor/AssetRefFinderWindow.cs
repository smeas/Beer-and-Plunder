/*
 * Author: Jonatan Johansson
 * Updated: 2021-04-26
 */

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class AssetRefFinderWindow : EditorWindow {
	[MenuItem("Assets/Find asset references", true)]
	private static bool FindAllAssetRefsValidate() {
		return Selection.objects.Length > 0;
	}

	[MenuItem("Assets/Find asset references")]
	private static void FindAllAssetRefs() {
		Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();

		foreach (Object o in Selection.objects) {
			string assetPath = AssetDatabase.GetAssetPath(o);
			if (string.IsNullOrEmpty(assetPath))
				continue;

			dict.Add(assetPath, new List<string>());
		}

		if (dict.Count == 0) return;

		foreach (string path in AssetDatabase.GetAllAssetPaths()) {
			string[] deps = AssetDatabase.GetDependencies(path, false);
			foreach (string key in dict.Keys) {
				if (key == path) continue;
				if (deps.Contains(key)) {
					dict[key].Add(path);
				}
			}
		}

		foreach (List<string> list in dict.Values)
			list.Sort();

		AssetRefFinderWindow window = CreateWindow<AssetRefFinderWindow>();
		window.titleContent = new GUIContent("Asset References");
		window.UpdateData(dict);
		window.Show();
	}


	[SerializeField] private (string asset, List<string> references)[] data;
	[SerializeField] private Vector2 scrollPosition;

	private void OnGUI() {
		scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
		EditorGUI.BeginDisabledGroup(true);

		foreach ((string asset, List<string> references) in data) {
			EditorGUILayout.ObjectField(AssetDatabase.LoadAssetAtPath<Object>(asset), typeof(Object), false);
			EditorGUI.indentLevel++;

			if (references.Count > 0) {
				foreach (string reference in references)
					EditorGUILayout.ObjectField(AssetDatabase.LoadAssetAtPath<Object>(reference), typeof(Object), false);
			}
			else {
				GUILayout.Label("No references");
			}

			EditorGUI.indentLevel--;
			EditorGUILayout.Space(EditorGUIUtility.singleLineHeight / 2);
		}

		EditorGUI.EndDisabledGroup();
		EditorGUILayout.EndScrollView();
	}

	private void UpdateData(Dictionary<string, List<string>> dataDict) {
		data = dataDict.Select(pair => (pair.Key, pair.Value)).ToArray();
	}
}