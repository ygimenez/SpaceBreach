using System.Collections.Generic;
using Godot;
using SpaceBreach.entity.interfaces;
using SpaceBreach.entity.model;

namespace SpaceBreach.entity.projectile.splash {
	public abstract class PlayerExplosion : Projectile, ISplash {
		private readonly HashSet<Entity> _hits = new HashSet<Entity>();

		protected PlayerExplosion() : base(speed: 0, damage: 100) {
		}

		public override void _Ready() {
			Scale = Vector2.One * (0.5f + Damage * 0.5f / 100);
		}

		protected override void OnHit(Entity entity) {
			if (_hits.Add(entity)) {
				entity.AddHp(Source, -Damage);
			}
		}
	}
}
