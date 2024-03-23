using Godot;
using SpaceBreach.entity.model;

namespace SpaceBreach.entity.projectile {
	public abstract class PlayerBullet : Projectile {
		[Export]
		public float Speed = 2;

		[Export]
		public float Damage = 50;

		public override void _PhysicsProcess(float delta) {
			GlobalTranslate(Vector2.Up.Rotated(Rotation) * Speed);
		}

		protected override void _Hit(RID areaRid, Area2D area, int areaShapeIdx, int localShapeIdx) {
			GD.Print("mogus");
		}
	}
}
