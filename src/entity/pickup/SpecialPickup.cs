using SpaceBreach.entity.model;

namespace SpaceBreach.entity.pickup {
	public abstract class SpecialPickup : Pickup {
		protected override void OnPickup(Player p) {
			p.SpCd.Reset();
		}
	}
}
