using Godot;
using SpaceBreach.entity.model;
using SpaceBreach.entity.projectile.splash;
using SpaceBreach.manager;
using SpaceBreach.util;

namespace SpaceBreach.entity.projectile {
	public abstract class PlayerBomb : Projectile {
		private float _fuse = 50;

		protected PlayerBomb() : base(speed: 3, damage: 0) {
		}

		public override void _Process(float delta) {
			base._Process(delta);

			if ((_fuse -= Engine.TimeScale) == 0) {
				OnHit(null);
			}
		}

		protected override void OnHit(Entity entity) {
			GetParent().CallDeferred("add_child", Poll<PlayerExplosion>(false).With(p => {
				p.Source = Source;
				p.Position = Position;
				p.Damage += (uint) (p.Damage * (1 - _fuse / 50));
			}));

			Release();
			Audio.Cue("res://assets/sounds/bomb_explode.wav");
		}
	}
}
