using System.Linq;
using Godot;
using SpaceBreach.entity.interfaces;
using SpaceBreach.entity.model;
using SpaceBreach.manager;
using SpaceBreach.util;

namespace SpaceBreach.scene {
	public abstract class Main : Control {
		static Main() {
			var subs = typeof(Projectile).Assembly
				.GetTypes()
				.Where(t => t.IsSubclassOf(typeof(Projectile)))
				.ToList();

			foreach (var type in subs) {
				if (type.Namespace != null) {
					if (typeof(ISplash).IsAssignableFrom(type)) {
						continue;
					}

					if (type.Namespace.ToLower().Contains("player")) {
						Projectile.Preload(type, 64);
						continue;
					}
				}

				Projectile.Preload(type, 1024);
			}
		}

		public override async void _Ready() {
			new ConfigFile().With(ed => ed.Load("res://export_presets.cfg"));
			GetNode<Label>("Version").Text = Global.VERSION;

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
			GetTree().Append("res://src/scene/Hangar.tscn");
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
