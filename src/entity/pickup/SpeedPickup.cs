using SpaceBreach.entity.model;

namespace SpaceBreach.entity.pickup {
	public abstract class SpeedPickup : Pickup {
		protected override void OnPickup(Player p) {
			p.RawSpeed += 0.05f;
		}
	}
}
