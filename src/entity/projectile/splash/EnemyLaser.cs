using SpaceBreach.entity.model;

namespace SpaceBreach.entity.projectile.splash {
	public abstract class EnemyLaser : Projectile {
		protected EnemyLaser() : base(speed: 0, damage: 100) {
		}
	}
}
