using Godot;
using SpaceBreach.entity.model;
using SpaceBreach.entity.projectile.splash;
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
				OnEntityHit(null);
			}
		}

		protected override void OnEntityHit(Entity entity) {
			GetParent().CallDeferred("add_child", Poll<PlayerExplosion>(false).With(p => {
				p.Source = Source;
				p.Position = Position;
				p.Damage += (uint) (p.Damage * (1 - _fuse / 150));
			}));

			Release();
			Audio.Cue("res://assets/sounds/bomb_explode.wav");
		}
	}
}
