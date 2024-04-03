using Godot;
using SpaceBreach.entity.model;
using SpaceBreach.util;

namespace SpaceBreach.entity.pickup {
	public class ChronoPickup : Pickup {
		public override void _Ready() {
			base._Ready();
			if (Engine.TimeScale < 1) {
				GetParent().AddChild(GD.Load<PackedScene>("res://src/entity/pickup/HpPickup.tscn").Instance<Pickup>().With(p =>
					p.Position = Position
				));
				QueueFree();
			}
		}

		protected override async void OnPickup(Player p) {
			Engine.TimeScale = 0.25f;
			await Game.Delay(10_000);
			Engine.TimeScale = 1f;
		}
	}
}
