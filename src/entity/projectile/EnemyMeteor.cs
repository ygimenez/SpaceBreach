using Godot;
using SpaceBreach.entity.interfaces;
using SpaceBreach.entity.model;

namespace SpaceBreach.entity.projectile {
	public abstract class EnemyMeteor : Projectile, ITracked {
		[Export]
		public Vector2 Target = -Vector2.One;

		private Vector2 _initialPos;
		private float _movFac;

		protected EnemyMeteor() : base(speed: 1.5f, damage: 150) {
		}

		public override void _Ready() {
			_initialPos = Position;
		}

		public override void _Process(float delta) {
			if (Target != -Vector2.One) {
				Speed = 0;
				_movFac = Mathf.Clamp(_movFac + 0.005f, 0, 1);
				Position = _initialPos.LinearInterpolate(Target, _movFac);

				if (_movFac >= 1) {
					Release();
				}
			}

			base._Process(delta);
		}
	}
}
