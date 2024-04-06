using Godot;
using SpaceBreach.util;

namespace SpaceBreach.manager {
	public class Audio : Node {
		private static readonly Audio Instance = new Audio();

		public static void PlayMusic(string path) {
			var master = Global.Cfg.GetV("vol_master", 100) / 100f;
			var music = Global.Cfg.GetV("vol_music", 50) / 100f;
			var volume = music * master;
			if (volume == 0) return;

			AudioStreamPlayer player;
			if (Global.Instance.HasNode("Music")) {
				player = Global.Instance.GetNode<AudioStreamPlayer>("Music");
				player.Stop();
			} else {
				player = new AudioStreamPlayer { Name = "Music" };
				Global.Instance.AddChild(player);
			}

			player.Stream = GD.Load<AudioStreamSample>(path);
			player.VolumeDb = GD.Linear2Db(music * master);

			player.Play();
		}

		public static void StopMusic() {
			if (Global.Instance.HasNode("Music")) {
				var player = Global.Instance.GetNode<AudioStreamPlayer>("Music");

				player.Stop();
				player.QueueFree();
			}
		}

		public static void TransitionMusic(string to, uint ramp = 0) {
			var master = Global.Cfg.GetV("vol_master", 100) / 100f;
			var music = Global.Cfg.GetV("vol_music", 50) / 100f;
			var volume = music * master;

			AudioStreamPlayer player;
			if (Global.Instance.HasNode("Music")) {
				player = Global.Instance.GetNode<AudioStreamPlayer>("Music");
				player.Stop();

				volume = GD.Db2Linear(player.VolumeDb);
			} else {
				player = new AudioStreamPlayer { Name = "Music" };
				player.VolumeDb = GD.Linear2Db(0);
				Global.Instance.AddChild(player);
			}

			var tween = player.CreateTween();
			tween.TweenProperty(player, "volume_db", GD.Linear2Db(0), ramp);
			tween.TweenCallback(player, "stop");
			tween.TweenProperty(player, "stream", GD.Load<AudioStreamSample>(to), 0);
			tween.TweenCallback(player, "play");
			tween.TweenProperty(player, "volume_db", volume, ramp);
		}

		public static void Cue(string path) {
			var master = Global.Cfg.GetV("vol_master", 100) / 100f;
			var effect = Global.Cfg.GetV("vol_effect", 75) / 100f;
			var volume = effect * master;
			if (volume == 0) return;

			var cue = new AudioStreamPlayer {
				Stream = GD.Load<AudioStreamSample>(path),
				VolumeDb = GD.Linear2Db(effect * master)
			};

			Global.Instance.AddChild(cue);
			cue.Play();
			cue.Connect("finished", cue, "queue_free");
		}

		public static void AttachUiAudio(Node root) {
			foreach (var child in root.GetChildren()) {
				if (child is Button b) {
					b.Connect("mouse_entered", Instance, nameof(_MouseOver));
					b.Connect("pressed", Instance, nameof(_MouseClick));
				}
			}
		}

		private void _MouseOver() {
			Cue("res://assets/sounds/ui/ui_hover.wav");
		}

		private void _MouseClick() {
			Cue("res://assets/sounds/ui/ui_click.wav");
		}
	}
}
