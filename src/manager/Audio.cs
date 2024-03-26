using Godot;
using SpaceBreach.util;

namespace SpaceBreach.manager {
	public class Audio : Node {
		private static readonly Audio Instance = new Audio();

		public static void Cue(Node parent, string path) {
			var master = Global.Cfg.GetV<int>("vol_master") / 100f;
			var effect = Global.Cfg.GetV<int>("vol_effect") / 100f;

			var cue = new AudioStreamPlayer {
				Stream = GD.Load<AudioStreamSample>(path),
				VolumeDb = GD.Linear2Db(effect * master)
			};

			parent.AddChild(cue);
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
			Cue(Global.Instance, "res://assets/sounds/ui/ui_hover.wav");
		}

		private void _MouseClick() {
			Cue(Global.Instance, "res://assets/sounds/ui/ui_click.wav");
		}
	}
}
