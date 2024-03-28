using Godot;
using SpaceBreach.entity.interfaces;
using SpaceBreach.entity.model;
using SpaceBreach.util;

namespace SpaceBreach.entity.misc {
	public class Marker : Node2D {
		[Export]
		public Entity Tracked;

		public override void _Ready() {
			Visible = false;
			if (Tracked is IBoss) {
				Scale *= 2;
				Modulate = Colors.Red;
			}
		}

		public override void _PhysicsProcess(float delta) {
			if (!IsInstanceValid(Tracked) || Tracked.Visible) {
				QueueFree();
			} else {
				var sprite = GetNode<Sprite>("Sprite");
				var reference = this.FindParent<Control>().GetGlobalRect();

				GlobalPosition = Tracked.GlobalPosition.Clamp(reference.Position, reference.End);
				sprite.Rotation = GlobalPosition.AngleTo(Tracked.GlobalPosition);

				var skull = sprite.GetNode<Sprite>("Skull");
				skull.Visible = Tracked is IBoss;
				skull.Rotation = -sprite.Rotation;
				Visible = true;
			}
		}
	}
}
