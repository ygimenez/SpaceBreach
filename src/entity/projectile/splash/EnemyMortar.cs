using System.Collections.Generic;
using SpaceBreach.entity.model;

namespace SpaceBreach.entity.projectile.splash {
	public abstract class EnemyMortar : Projectile {
		private readonly HashSet<Entity> _hits = new HashSet<Entity>();

		protected EnemyMortar() : base(speed: 0, damage: 125) {
		}

		protected override void OnHit(Entity entity) {
			if (_hits.Add(entity)) {
				entity.AddHp(Source, -Damage);
			}
		}
	}
}
