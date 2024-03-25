using Godot;

namespace SpaceBreach.entity.model {
	public abstract class Projectile : Area2D {
		[Export]
		public float Speed = 2;

		[Export]
		public float Damage = 50;

		protected Projectile(float speed, float damage) {
			Speed = speed;
			Damage = damage;
		}

		public override void _Ready() {
			Connect("area_shape_entered", this, nameof(_Hit));
		}

		public override void _PhysicsProcess(float delta) {
			GlobalTranslate(Vector2.Up.Rotated(Rotation) * Speed);
		}

		protected virtual void _Hit(RID areaRid, Area2D area, int areaShapeIdx, int localShapeIdx) {
			if (area is Entity e) {

			}
		}
	}
}
