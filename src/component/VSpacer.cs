using Godot;

namespace SpaceBreach.component {
	public class VSpacer : Control {
		public VSpacer() {
			SizeFlagsVertical = (int) SizeFlags.ExpandFill;
		}
	}
}
