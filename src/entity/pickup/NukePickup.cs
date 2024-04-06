using SpaceBreach.entity.model;

namespace SpaceBreach.entity.pickup {
	public abstract class NukePickup : Pickup {
		protected override void OnPickup(Player p) {
			Game.Nuke();
		}
	}
}
