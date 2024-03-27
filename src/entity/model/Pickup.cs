using Godot;
using SpaceBreach.manager;

namespace SpaceBreach.entity.model {
	public abstract class Pickup : Area2D {
		public override void _Ready() {
			Connect("area_entered", this, nameof(_AreaEntered));
		}

		public override void _PhysicsProcess(float delta) {
			Translate(Vector2.Down * 0.2f);
		}

		public void _AreaEntered(Area2D entity) {
			if (entity is Player p) {
				OnPickup(p);
				QueueFree();
				Audio.Cue("res://assets/sounds/pickup.wav");
			}
		}

		protected abstract void OnPickup(Player p);
	}
}
