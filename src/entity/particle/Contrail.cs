using Godot;
using SpaceBreach.entity.model;
using SpaceBreach.util;

namespace SpaceBreach.entity.particle {
	public abstract class Contrail : Position2D {
		[Export]
		public int Length = 5;

		[Export]
		public Vector2 Velocity = Vector2.Zero;

		private Player _ship;
		private Line2D _line;

		public override void _Ready() {
			_ship = this.FindParent<Player>();
			_line = GetNode<Line2D>("Line2D");
			_line.SetAsToplevel(true);
		}

		public override void _Process(float delta) {
			if (_line == null || _ship == null) return;

			_line.AddPoint(GlobalPosition);
			while (_line.GetPointCount() > Length / Engine.TimeScale) {
				_line.RemovePoint(0);
			}

			for (var i = 0; i < _line.GetPointCount(); i++) {
				_line.SetPointPosition(i, _line.GetPointPosition(i) + Velocity * _ship.Speed / 2 * delta);
			}
		}
	}
}
