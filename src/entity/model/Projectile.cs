using Godot;

namespace SpaceBreach.entity.model {
	public abstract class Projectile : Entity {
		public override void _Ready() {
			Connect("area_shape_entered", this, nameof(_Hit));
		}

		protected abstract void _Hit(RID areaRid, Area2D area, int areaShapeIdx, int localShapeIdx);
	}
}
