using SpaceBreach.entity.model;

namespace SpaceBreach.entity.pickup {
	public class SpeedPickup : Pickup {
		protected override void OnPickup(Player p) {
			p.RawSpeed += 0.05f;
		}
	}
}
