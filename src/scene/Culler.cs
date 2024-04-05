using Godot;
using SpaceBreach.entity.model;
using SpaceBreach.util;

namespace SpaceBreach.scene {
	public abstract class Culler : Area2D {
		public override void _Ready() {
			Connect("area_exited", this, nameof(_AreaExited));
		}

		public void _AreaExited(Area2D entity) {
			var viewport = GetNode<Game>("/root/Control").GetGlobalRect();
			var entPos = entity.GlobalPosition;
			if (entPos >= viewport.Position && entPos <= viewport.End) return;

			if (entity is Projectile p) {
				p.Release();
			} else {
				entity.QueueFree();
			}
		}
	}
}
