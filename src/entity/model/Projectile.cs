using Godot;
using SpaceBreach.util;

namespace SpaceBreach.entity.model {
	public abstract class Projectile : Area2D {
		[Export]
		public float Speed;

		[Export]
		public uint Damage;

		public Entity Source;

		protected Projectile(float speed, uint damage) {
			Speed = speed;
			Damage = damage;
		}

		public override void _PhysicsProcess(float delta) {
			if (Speed != 0) {
				GlobalTranslate(Vector2.Up.Rotated(Rotation) * Speed * Global.ACTION_SPEED);
			}

			foreach (var area in GetOverlappingAreas()) {
				if (area is Entity e) {
					OnHit(e);
				}
			}
		}

		protected virtual void OnHit(Entity entity) {
			if (entity != null && Damage > 0) {
				entity.AddHp(Source, -Damage);
			}

			QueueFree();
		}
	}
}
