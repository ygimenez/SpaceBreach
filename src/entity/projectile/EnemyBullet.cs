using SpaceBreach.entity.model;

namespace SpaceBreach.entity.projectile {
	public abstract class EnemyBullet : Projectile {
		protected EnemyBullet() : base(speed: 1, damage: 50) {
		}
	}
}
