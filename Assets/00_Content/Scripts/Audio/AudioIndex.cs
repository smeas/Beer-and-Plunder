namespace Audio {
	public static partial class AudioIndex {
		public static string GetPath(SoundEffect effect) {
			return soundEffectPaths[(int)effect];
		}
	}
}