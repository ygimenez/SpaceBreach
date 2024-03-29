using Godot;
using SpaceBreach.manager;
using SpaceBreach.util;

namespace SpaceBreach.scene {
	public abstract class Main : Control {
		public override async void _Ready() {
			new ConfigFile().With(ed => {
				ed.Load("res://export_presets.cfg");

				var version = ed.GetValue("preset.0.options", "application/product_version", "0.0.0-DEV");
				GetNode<Label>("Version").Text = version.ToString();
			});

			if (Global.Mobile) {
				GetNode<Button>("Fullscreen").Connect("pressed", this, nameof(_FullscreenPressed));
			} else {
				GetNode<Button>("Fullscreen").QueueFree();
			}

			GetNode<Button>("Play").Connect("pressed", this, nameof(_PlayPressed));
			GetNode<Button>("Settings").Connect("pressed", this, nameof(_SettingsPressed));
			GetNode<Button>("Exit").With(b => {
				b.Connect("pressed", this, nameof(_ExitPressed));

				if (OS.HasFeature("web")) {
					b.Visible = false;
				}
			});

			GetNode<Button>("Leaderboard").Connect("pressed", this, nameof(_LeaderboardPressed));

			Audio.AttachUiAudio(this);
			await Global.LoadLeaderboard();
		}

		public override void _Process(float delta) {
			GetNode<Button>("Leaderboard").Disabled = !Global.Online;
		}

		private void _FullscreenPressed() {
			OS.WindowFullscreen = !OS.WindowFullscreen;
		}

		private void _PlayPressed() {
			GetTree().Append("res://src/scene/Game.tscn");
		}

		private void _LeaderboardPressed() {
			GetTree().Append("res://src/scene/Leaderboard.tscn");
		}

		private void _SettingsPressed() {
			GetTree().Append("res://src/scene/Settings.tscn");
		}

		private void _ExitPressed() {
			GetTree().Quit();
		}
	}
}
