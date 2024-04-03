using Godot;
using SpaceBreach.entity.model;

namespace SpaceBreach.entity.pickup {
	public class SlowPickup : Pickup {
		protected override void OnPickup(Player p) {
			p.RawSpeed = Mathf.Min(0.1f, p.RawSpeed - 0.1f);
		}
	}
}
