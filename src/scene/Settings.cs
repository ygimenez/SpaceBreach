using Godot;
using SpaceBreach.manager;

namespace SpaceBreach.scene;

public partial class Settings : Control {
	public override void _Ready() {
		GetNode<Button>("Back").Pressed += GetTree().Pop;
	}
}
