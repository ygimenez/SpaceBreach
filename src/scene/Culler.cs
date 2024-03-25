using Godot;

namespace SpaceBreach.scene {
	public abstract class Culler : Area2D {
		public override void _Ready() {
			Connect("area_exited", this, nameof(_AreaExited));
			((RectangleShape2D) GetNode<CollisionShape2D>("CollisionShape2D").Shape).Extents = GetParent<Control>().RectSize / 2;
		}

		public void _AreaExited(Area2D entity) {
			entity.QueueFree();
		}
	}
}
