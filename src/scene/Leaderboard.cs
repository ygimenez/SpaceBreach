using Godot;
using SpaceBreach.util;

namespace SpaceBreach.scene {
	public abstract class Leaderboard : BaseMenu {
		public override async void _Ready() {
			base._Ready();
			await Global.LoadLeaderboard();

			GetNode<GridContainer>("MaxSizeContainer/ScrollContainer/Rows").With(box => {
				foreach (var e in Global.Leaderboard) {
					box.AddChild(new AutoFont {
						Text = $"{e.Item1} ",
						TestString = "##### #####",
						SizeFlagsHorizontal = (int) SizeFlags.ExpandFill,
						Align = Label.AlignEnum.Right,
                        FontSize = 30
					});
					box.AddChild(new AutoFont {
						Text = $" {e.Item2}",
						TestString = "##### #####",
						SizeFlagsHorizontal = (int) SizeFlags.ExpandFill,
                        FontSize = 30
					});
				}
			});
		}
	}
}
