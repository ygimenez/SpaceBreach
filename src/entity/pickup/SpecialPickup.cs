using SpaceBreach.entity.model;

namespace SpaceBreach.entity.pickup {
	public class SpecialPickup : Pickup {
		protected override void OnPickup(Player p) {
			p.SpCd.Reset();
		}
	}
}
