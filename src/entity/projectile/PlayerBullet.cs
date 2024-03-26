using Godot;
using SpaceBreach.entity.model;

namespace SpaceBreach.entity.projectile {
	public abstract class PlayerBullet : Projectile {
		protected PlayerBullet() : base(speed: 2, damage: 50) {
		}
	}
}
