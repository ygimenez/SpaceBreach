using Godot;
using SpaceBreach.entity.model;

namespace SpaceBreach.entity.projectile {
	public class PlayerBullet : Projectile {
		[Export]
		public float Speed = 1;

		[Export]
		public float Damage = 50;

		public override void _PhysicsProcess(float delta) {
			Translate(Vector2.Up.Rotated(Rotation));
		}

		protected override void _Hit(RID areaRid, Area2D area, int areaShapeIdx, int localShapeIdx) {

		}
	}
}
