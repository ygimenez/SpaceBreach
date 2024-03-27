using Godot;

namespace SpaceBreach.component {
	public class ConstrainedContainer : Container {
		[Export]
		public Vector2 MaxSize = Vector2.Zero;

		public override void _Process(float delta) {
			var offset = RectSize;

			RectSize = new Vector2(
				MaxSize.x > 0 ? Mathf.Min(RectSize.x, MaxSize.x) : RectSize.x,
				MaxSize.y > 0 ? Mathf.Min(RectSize.y, MaxSize.y) : RectSize.y
			);

			offset -= RectSize;
			MarginLeft += offset.x;
			MarginTop += offset.y;
		}
	}
}
