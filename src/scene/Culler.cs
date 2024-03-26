using Godot;

namespace SpaceBreach.scene {
	public abstract class Culler : Area2D {
		public override void _Ready() {
			Connect("area_exited", this, nameof(_AreaExited));
		}

		public void _AreaExited(Area2D entity) {
			entity.QueueFree();
		}
	}
}
