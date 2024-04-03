using SpaceBreach.entity.model;

namespace SpaceBreach.entity.pickup {
	public class NukePickup : Pickup {
		protected override void OnPickup(Player p) {
			Game.Nuke();
		}
	}
}
