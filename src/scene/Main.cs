using Godot;
using SpaceBreach.manager;
using SpaceBreach.util;

namespace SpaceBreach.scene;

public partial class Main : Control {
	public override void _Ready() {
		new ConfigFile().With(ed => {
			ed.Load("res://export_presets.cfg");

			var version = ed.GetValue("preset.0.options", "application/product_version", "0.0.0-DEV");
			GetNode<Label>("Version").Text = version.AsString();
		});

		GetNode<Button>("Settings").Pressed += () => GetTree().Append("res://src/scene/Settings.tscn");
		GetNode<Button>("Exit").Pressed += GetTree().Quit;
	}
}
