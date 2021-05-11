using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Audio;
using UnityEditor;
using UnityEngine;

public class AudioIndexGenerator {
	private const string ResourcePrefix = "Assets/Resources/";
	private const string SoundEffectsResourcePath = "Audio/SoundEffects/";
	private const string GeneratedFilePath = "00_Content/Scripts/Audio/AudioIndex.Generated.cs";

	private const string Template = @"
using System.Collections.Generic;

namespace Audio {{
	public enum SoundEffect {{
{0}
	}}

	public static partial class AudioIndex {{
		private static Dictionary<int, string> soundEffectPaths = new Dictionary<int, string> {{
{1}
		}};
	}}
}}";

	[MenuItem("Tools/Generate Audio Index")]
	public static void GenerateIndex() {
		SoundCue[] soundCues = Resources.LoadAll<SoundCue>(SoundEffectsResourcePath);
		string[] resourcePaths = soundCues.Select(GetResourcePath).ToArray();
		int[] assetIds = soundCues.Select(GetAssetGuidHash).ToArray();

		string enumContent =
			string.Join(",\r\n",
			            resourcePaths.Zip(assetIds, (path, id) => $"{MakeSafeNameFromPath(path)} = {id}")
				            .Select(s => Indent(s, 2)));

		string dictionaryContent =
			string.Join(",\r\n",
			            resourcePaths
				            .Zip(assetIds, (path, id) => $"{{ {id}, {Quote(path)} }}")
				            .Select(str => Indent(str, 3)));

		string generated = string.Format(Template, enumContent, dictionaryContent);

		string outputPath = Path.Combine(Application.dataPath, GeneratedFilePath);
		File.WriteAllText(outputPath, generated, Encoding.UTF8);
		AssetDatabase.Refresh();
	}

	private static string GetResourcePath(Object asset) {
		string assetPath = AssetDatabase.GetAssetPath(asset);

		Debug.Assert(assetPath != null, "assetPath != null");
		Debug.Assert(assetPath.StartsWith(ResourcePrefix));

		assetPath = Path.ChangeExtension(assetPath, null);
		return assetPath.Substring(ResourcePrefix.Length);
	}

	private static int GetAssetGuidHash(Object asset) {
		string assetPath = AssetDatabase.GetAssetPath(asset);
		return AssetDatabase.GUIDFromAssetPath(assetPath).GetHashCode();
	}

	private static string MakeSafeNameFromPath(string path) {
		Debug.Assert(path.StartsWith(SoundEffectsResourcePath));
		string rawName = path.Substring(SoundEffectsResourcePath.Length);

		string safeName = rawName.Replace('/', '_');
		safeName = Regex.Replace(safeName, "[^A-Za-z0-9_]", "");
		Debug.Assert(safeName.Length > 0, "No safe chars in name");

		if (char.IsDigit(safeName[0]))
			safeName = '_' + safeName;

		return safeName;
	}

	private static string Quote(string str) => '"' + str + '"';
	private static string Indent(string str, int amount = 1) => new string('\t', amount) + str;
}