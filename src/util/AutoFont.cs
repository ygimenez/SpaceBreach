using Godot;

namespace SpaceBreach.util {
	public class AutoFont : Label {
		public override void _Process(float delta) {
			var rect = RectSize;
			var box = this.FindParent<Control>().RectSize;

			var scale = 1f;
			if (rect.x > box.x) {
				scale = box.x / rect.x;
			}

			if (rect.y > box.y) {
				scale = Mathf.Min(box.y / rect.y, scale);
			}

			RectScale = new Vector2(scale, scale);
		}
	}
}
