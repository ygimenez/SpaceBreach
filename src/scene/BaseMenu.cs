using Godot;
using SpaceBreach.manager;

namespace SpaceBreach.scene {
	public abstract class BaseMenu : Control {
		public override void _Ready() {
			GetNode<Button>("Back").Connect("pressed", this, nameof(_BackPressed));
			Audio.AttachUiAudio(this);
		}

		public void _BackPressed() {
			GetTree().Pop();
		}
	}
}
