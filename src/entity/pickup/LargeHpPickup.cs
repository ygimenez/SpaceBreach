using SpaceBreach.entity.model;

namespace SpaceBreach.entity.pickup {
	public abstract class LargeHpPickup : Pickup {
		protected override void OnPickup(Player p) {
			p.BaseHp += 50;
			p.AddHp(null, p.BaseHp / 2);
		}
	}
}
