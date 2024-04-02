using Godot;
using SpaceBreach.util;

namespace SpaceBreach.scene {
	public class BossHp : ProgressBar {
		public override void _Process(float delta) {
			var game = this.FindParent<Game>();

			Visible = game.Boss != null;
			MaxValue = game.Boss?.BaseHp ?? 100;
		}
	}
}
