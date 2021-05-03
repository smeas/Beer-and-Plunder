namespace Vikings {
	public enum DesireType {
		Lager,
		Stout,
		Ale,
		Harp,
	}

	public static class DesireTypeExtensions {
		public static bool IsBeer(this DesireType type) {
			return type switch {
				DesireType.Lager => true,
				DesireType.Stout => true,
				DesireType.Ale => true,
				_ => false
			};
		}
	}
}
