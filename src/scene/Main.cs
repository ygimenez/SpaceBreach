using Godot;
using SpaceBreach.manager;
using SpaceBreach.util;

namespace SpaceBreach.scene {
	public class Main : Control {
		public override void _Ready() {
			new ConfigFile().With(ed => {
				ed.Load("res://export_presets.cfg");

				var version = ed.GetValue("preset.0.options", "application/product_version", "0.0.0-DEV");
				GetNode<Label>("Version").Text = version.ToString();
			});

			GetNode<Button>("Settings").Connect("pressed", this, nameof(_SettingsPressed));
			GetNode<Button>("Exit").Connect("pressed", this, nameof(_ExitPressed));
		}

		private void _SettingsPressed() {
			GetTree().Append("res://src/scene/Settings.tscn");
		}

		private void _ExitPressed() {
			GetTree().Quit();
		}
	}
}
