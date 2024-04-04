using Godot;
using SpaceBreach.entity.model;

namespace SpaceBreach.entity.projectile {
	public abstract class PlayerBullet : Projectile {
		protected PlayerBullet() : base(speed: 5, damage: 50) {
		}
	}
}
