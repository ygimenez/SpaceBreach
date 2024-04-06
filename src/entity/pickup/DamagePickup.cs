using SpaceBreach.entity.model;

namespace SpaceBreach.entity.pickup {
	public abstract class DamagePickup : Pickup {
		protected override void OnPickup(Player p) {
			p.DamageMult += 0.1f;
		}
	}
}
