using Godot;
using SpaceBreach.entity.model;
using SpaceBreach.util;

namespace SpaceBreach.entity.projectile {
	public abstract class PlayerBomb : Projectile {
		protected PlayerBomb() : base(speed: 1, damage: 0) {

		}

		protected override void OnHit(Entity entity) {
			var explode = GD.Load<PackedScene>("res://src/entity/projectile/splash/PlayerExplosion.tscn");

			GetParent().CallDeferred("add_child", explode.Instance<Projectile>().With(p => {
				p.Source = Source;
				p.Position = Position;
			}));
			QueueFree();
		}
	}
}
