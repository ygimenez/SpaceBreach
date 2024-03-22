using Godot;
using SpaceBreach.manager;

namespace SpaceBreach.scene {
	public abstract class BaseMenu : Control {
		public override void _Ready() {
			GetNode<Button>("Back").Connect("pressed", this, nameof(_BackPressed));
		}

		protected void _BackPressed() {
			GetTree().Pop();
		}
	}
}
