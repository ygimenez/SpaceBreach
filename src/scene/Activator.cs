using Godot;
using SpaceBreach.entity.model;

namespace SpaceBreach.scene {
	public abstract class Activator : Area2D {
		public override void _Ready() {
			Connect("area_entered", this, nameof(_AreaEntered));
			Connect("area_shape_exited", this, nameof(_AreaShapeExited));
		}

		public void _AreaEntered(Area2D entity) {
			if (entity is Entity e) {
				e.Visible = true;
			}
		}

		public void _AreaShapeExited(RID _, Area2D entity, int __, int index) {
			if (entity is Enemy e && e.Visible && GetChild(index).Name == "Bottom") {
				GetNode<Game>("/root/Control").Streak = 0;
			}
		}
	}
}
