using SpaceBreach.entity.interfaces;
using SpaceBreach.entity.model;

namespace SpaceBreach.entity.projectile {
	public abstract class EnemyMeteor : Projectile {
		protected EnemyMeteor() : base(speed: 0.1f, damage: 150) {
		}

		public override void _Ready() {
			var world = GetSafeArea().GetNode<Node2D>("World");
			world.AddChild(GD.Load<PackedScene>("res://src/entity/misc/Marker.tscn").Instance<Marker>().With(m => {
				m.Tracked = e;
			}));
		}
	}
}
