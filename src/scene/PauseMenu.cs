using Godot;
using SpaceBreach.manager;

namespace SpaceBreach.scene {
	public abstract class PauseMenu : Control {
		public override void _Ready() {
			GetNode<Button>("Back").Connect("pressed", this, nameof(_BackPressed));
		}

		public override void _Input(InputEvent @event) {
			if (@event.IsActionPressed("pause")) {
				var cd = GetNode<Label>("../Countdown");
				var anim = cd.GetNode<AnimationPlayer>("AnimationPlayer");
				anim.Stop();

				if (Visible) {
					anim.Play("Countdown");
					cd.Visible = true;
				} else {
					GetTree().Paused = true;
					cd.Visible = false;
				}

				Visible = !Visible;
			}
		}

		public void _BackPressed() {
			GetTree().Pop();
		}

		public void _Unpause() {
			GetTree().Paused = false;
			GetNode<Label>("../Countdown").Visible = false;
		}
	}
}
