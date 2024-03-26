using SpaceBreach.entity.model;

namespace SpaceBreach.entity.pickup {
	public class ProjPickup : Pickup {
		protected override void OnPickup(Player p) {
			p.Projectiles++;
		}
	}
}
