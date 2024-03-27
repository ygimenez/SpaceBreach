using Godot;

namespace SpaceBreach.component {
	public class HSpacer : Control {
        [Export]
        public float Flex = 1;

		public HSpacer() {
			SizeFlagsHorizontal = (int) SizeFlags.ExpandFill;
            SizeFlagsStretchRatio = Flex;
		}
	}

	public class VSpacer : Control {
        [Export]
        public float Flex = 1;

		public VSpacer() {
			SizeFlagsVertical = (int) SizeFlags.ExpandFill;
            SizeFlagsStretchRatio = Flex;
		}
	}
}
