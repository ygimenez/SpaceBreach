using SpaceBreach.entity.model;

namespace SpaceBreach.entity.pickup {
	public abstract class ProjPickup : Pickup {
		protected override void OnPickup(Player p) {
			p.Projectiles++;
		}
	}
}
