using Godot;
using SpaceBreach.util;

namespace SpaceBreach.component {
	public class HFlexContainer : Container {
		[Export]
		public int Flex = 1;

		public override void _Process(float delta) {
			var parent = this.FindParent<Control>();
			if (parent == null) return;

			var totalFlex = 0;
			var available = parent.RectSize.x;
			foreach (var child in parent.GetChildren()) {
				switch (child) {
					case HFlexContainer f:
						totalFlex += f.Flex;
						break;
					case Control c:
						available -= c.RectSize.x;
						break;
				}
			}

			RectMinSize = new Vector2(Flex * available / totalFlex, 0);
		}
	}

	public class VFlexContainer : Container {
		[Export]
		public int Flex = 1;

		public override void _Process(float delta) {
			var parent = this.FindParent<Control>();
			if (parent == null) return;

			var totalFlex = 0;
			var available = parent.RectSize.y;
			foreach (var child in parent.GetChildren()) {
				switch (child) {
					case VFlexContainer f:
						totalFlex += f.Flex;
						break;
					case Control c:
						available -= c.RectSize.y;
						break;
				}
			}

			RectMinSize = new Vector2(0, Flex * available / totalFlex);
		}
	}
}
