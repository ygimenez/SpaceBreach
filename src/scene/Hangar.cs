using System.Linq;
using Godot;
using SpaceBreach.entity.model;
using SpaceBreach.util;

namespace SpaceBreach.scene {
	public class Hangar : BaseMenu {
		public override void _Ready() {
			base._Ready();

			var ships = typeof(Player).Assembly
				.GetTypes()
				.Where(t => t.IsSubclassOf(typeof(Player)))
				.Select(t => Utils.Load(t).Instance<Player>())
				.ToList();


		}
	}
}
