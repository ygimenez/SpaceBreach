using Godot;
using SpaceBreach.entity.model;

namespace SpaceBreach.entity.particle {
	public class Contrail : Position2D {
		[Export]
		public Player Ship;

		[Export]
		public int Length = 5;

		[Export]
		public Vector2 Velocity = Vector2.Zero;

		private Line2D _line;

		public override void _Ready() {
			_line = GetNode<Line2D>("Line2D");
			_line.SetAsToplevel(true);
		}

		public override void _Process(float delta) {
			if (_line == null || Ship == null) return;

			_line.AddPoint(GlobalPosition);
			if (_line.GetPointCount() > Length) {
				_line.RemovePoint(0);
			}

			for (var i = 0; i < _line.GetPointCount(); i++) {
				_line.SetPointPosition(i, _line.GetPointPosition(i) + Velocity * Ship.Speed * delta);
			}
		}
	}
}
