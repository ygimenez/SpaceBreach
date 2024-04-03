using Godot;
using SpaceBreach.manager;
using SpaceBreach.scene;

namespace SpaceBreach.entity.model {
	public abstract class Pickup : Area2D {
		protected Game Game => GetNode<Game>("/root/Control");

		public override void _Ready() {
			Connect("area_entered", this, nameof(_AreaEntered));
		}

		public override void _PhysicsProcess(float delta) {
			Translate(Vector2.Down * 0.3f);
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
