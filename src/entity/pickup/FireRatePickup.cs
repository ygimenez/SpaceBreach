using SpaceBreach.entity.model;

namespace SpaceBreach.entity.pickup {
	public class FireRatePickup : Pickup {
		protected override void OnPickup(Player p) {
			p.AttackRate += 0.2f;
		}
	}
}
