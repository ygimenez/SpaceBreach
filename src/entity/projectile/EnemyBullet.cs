using SpaceBreach.entity.model;

namespace SpaceBreach.entity.projectile {
	public abstract class EnemyBullet : Projectile {
		protected EnemyBullet() : base(speed: 2, damage: 50) {
		}
	}
}
