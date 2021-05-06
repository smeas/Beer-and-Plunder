namespace Audio {
	public static partial class AudioIndex {
		/// <summary>
		/// Get the resource path to the specified sound effect.
		/// </summary>
		public static string GetPath(SoundEffect effect) {
			return soundEffectPaths[(int)effect];
		}
	}
}