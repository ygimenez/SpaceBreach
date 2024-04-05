using Godot;
using SpaceBreach.entity.model;
using SpaceBreach.util;

namespace SpaceBreach.entity.particle {
	public abstract class Trail : Position2D {
		[Export]
		public int Length = 5;

		[Export]
		public Vector2 Velocity = Vector2.Zero;

		[Export]
		public Position2D Source;

		[Export]
		public Player Ship;

		private Line2D _line;

		public override void _Ready() {
			_line = GetNode<Line2D>("Line2D");
		}

		public override void _Process(float delta) {
			if (!Source.IsInsideTree()) {
				QueueFree();
				return;
			}

			if (_line == null || Source == null) return;

			_line.Visible = Source.Visible;
			if (!_line.Visible) return;

			_line.AddPoint(GetParent<Node2D>().ToLocal(Source.GlobalPosition));
			while (_line.GetPointCount() > Length / Engine.TimeScale) {
				_line.RemovePoint(0);
			}

			for (var i = 0; i < _line.GetPointCount(); i++) {
				_line.SetPointPosition(i, _line.GetPointPosition(i) + Velocity * (Ship?.Speed ?? 1) / 2 * delta);
			}
		}
	}
}
