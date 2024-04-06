using Godot;
using SpaceBreach.entity.interfaces;
using SpaceBreach.entity.model;
using SpaceBreach.util;

namespace SpaceBreach.entity.misc {
	public abstract class Marker : Node2D {
		[Export]
		public Node2D Tracked;

		public override void _Ready() {
			Visible = false;
			if (Tracked is IBoss) {
				Scale *= 2;
				Modulate = Colors.Red;
			}
		}

		public override void _PhysicsProcess(float delta) {
			if (!IsInstanceValid(Tracked) || Tracked is ITracked t && t.Appeared) {
				QueueFree();
			} else {
				var sprite = GetNode<Node2D>("Sprite");
				var constraint = this.FindParent<Control>().GetGlobalRect();

				GlobalPosition = Tracked.GlobalPosition.Clamp(constraint.Position, constraint.End);
				sprite.Rotation = Tracked.Position.AngleToPoint(Position);

				var skull = sprite.GetNode<Node2D>("Skull");
				skull.Visible = Tracked is IBoss;
				skull.Rotation = -sprite.Rotation;
				Visible = true;
			}
		}
	}
}
