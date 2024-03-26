using Godot;
using SpaceBreach.util;

namespace SpaceBreach.manager {
	public class Audio : Node {
		private static Audio _instance;

		public override void _Ready() {
			_instance = GetNode<Audio>("/root/Audio");
		}

		public static void Cue(Node parent, string path) {
			var master = Global.Cfg.GetV<int>("vol_master") / 100f;
			var effect = Global.Cfg.GetV<int>("vol_effect") / 100f;

			new AudioStreamPlayer().With(a => {
				a.Stream = ResourceLoader.Load<AudioStreamSample>(path);
				a.VolumeDb = GD.Linear2Db(effect * master);

				parent.AddChild(a);
				a.Play();
				a.Connect("finished", a, "queue_free");
			});
		}

		public static void AttachUiAudio(Node root) {
			foreach (var child in root.GetChildren()) {
				if (child is Button b) {
					b.Connect("mouse_entered", _instance, nameof(_MouseOver));
					b.Connect("pressed", _instance, nameof(_MouseClick));
				}
			}
		}

		private void _MouseOver() {
			Cue(_instance, "res://assets/sounds/ui/ui_hover.wav");
		}

		private void _MouseClick() {
			Cue(_instance, "res://assets/sounds/ui/ui_click.wav");
		}
	}
}
