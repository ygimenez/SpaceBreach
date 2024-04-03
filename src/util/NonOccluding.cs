using Godot;

namespace SpaceBreach.util {
	public class NonOccluding : Area2D {
		private Color _oldModulate;

		public override void _Ready() {
			this.AddCollision();
			_oldModulate = Modulate;
		}

		public override void _Process(float delta) {
			if (GetOverlappingAreas().Count == 0) {
				GetParent<Control>().Modulate = _oldModulate;
			} else {
				GetParent<Control>().Modulate = new Color(_oldModulate, 0.25f);
			}
		}
	}
}
