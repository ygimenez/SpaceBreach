using SpaceBreach.entity.model;

namespace SpaceBreach.entity.pickup {
	public class HpPickup : Pickup {
		protected override void OnPickup(Player p) {
			p.AddHp(null, (long) (p.BaseHp * 0.25f));
		}
	}
}
