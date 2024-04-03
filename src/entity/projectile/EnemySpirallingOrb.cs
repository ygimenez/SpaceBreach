using Godot;
using SpaceBreach.entity.model;

namespace SpaceBreach.entity.projectile {
	public abstract class EnemySpirallingOrb : Projectile {
		[Export]
		public float RotationSpeed;

		private Vector2 _epicenter;
		private float _angle, _radius;

		protected EnemySpirallingOrb() : base(speed: 3, damage: 10) {
		}

		public override void _Ready() {
			_epicenter = Position;
			_angle = RotationDegrees;
		}

		public override void _Process(float delta) {
			_angle += RotationSpeed;
			_radius += Speed;

			Position = _epicenter + new Vector2(
				Mathf.Cos(Mathf.Deg2Rad(_angle)) * _radius,
				Mathf.Sin(Mathf.Deg2Rad(_angle)) * _radius
			);
		}
	}
}
