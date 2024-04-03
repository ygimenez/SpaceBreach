using Godot;
using SpaceBreach.entity.model;
using SpaceBreach.manager;
using SpaceBreach.util;

namespace SpaceBreach.entity.projectile {
	public abstract class PlayerBomb : Projectile {
		private float _fuse = 150;

		protected PlayerBomb() : base(speed: 1, damage: 0) {
		}

		public override void _PhysicsProcess(float delta) {
			base._PhysicsProcess(delta);

			if ((_fuse -= Engine.TimeScale) == 0) {
				OnHit(null);
			}
		}

		protected override void OnHit(Entity entity) {
			var explode = GD.Load<PackedScene>("res://src/entity/projectile/splash/PlayerExplosion.tscn");

			GetParent().CallDeferred("add_child", explode.Instance<Projectile>().With(p => {
				p.Source = Source;
				p.Position = Position;
				p.Damage += (uint) (p.Damage * (1 - _fuse / 150f));
			}));
			QueueFree();
			Audio.Cue("res://assets/sounds/bomb_explode.wav");
		}
	}
}
