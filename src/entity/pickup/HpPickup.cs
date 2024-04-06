using SpaceBreach.entity.model;

namespace SpaceBreach.entity.pickup {
	public abstract class HpPickup : Pickup {
		protected override void OnPickup(Player p) {
			p.BaseHp += 20;
			p.AddHp(null, (long) (p.BaseHp * 0.25f));
		}
	}
}
