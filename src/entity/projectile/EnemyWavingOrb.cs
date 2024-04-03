using Godot;
using SpaceBreach.entity.model;

namespace SpaceBreach.entity.projectile {
	public abstract class EnemyWavingOrb : Projectile {
		private int _angle;

		protected EnemyWavingOrb() : base(speed: 0.8f, damage: 75) {
		}

		public override void _Process(float delta) {
			Speed = 0.8f * Mathf.Abs(Mathf.Sin(Mathf.Deg2Rad(_angle += 5)));
		}
	}
}
