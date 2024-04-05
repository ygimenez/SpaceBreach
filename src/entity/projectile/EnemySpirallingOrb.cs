using Godot;
using SpaceBreach.entity.model;
using SpaceBreach.util;

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

		public override void _PhysicsProcess(float delta) {
			_angle += RotationSpeed;
			_radius += Speed;

			Position = _epicenter + new Vector2(
				Utils.FCos(_angle) * _radius,
				Utils.FSin(_angle) * _radius
			);

			foreach (var area in GetOverlappingAreas()) {
				if (area is Entity e) {
					OnEntityHit(e);
				}
			}
		}
	}
}
