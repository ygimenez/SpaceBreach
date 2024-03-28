using Godot;
using SpaceBreach.manager;
using SpaceBreach.util;

namespace SpaceBreach.scene {
	public abstract class PauseMenu : Control {
		[Export]
		public new bool Visible {
			get => base.Visible;
			set {
				base.Visible = value;

				if (!GetParent<Game>().IsGameOver()) {
					_Pause();
				}
			}
		}

		public override void _Ready() {
			GetNode<Button>("Back").Connect("pressed", this, nameof(_BackPressed));
			Audio.AttachUiAudio(this);
		}

		public override void _Process(float delta) {
			if (!Global.Mobile) {
				Input.MouseMode = Visible ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured;
			}

			GetNode<Label>("Label").Text = GetParent<Game>().IsGameOver() ? "GAME OVER" : "PAUSED";
		}

		public override void _Input(InputEvent @event) {
			if (GetParent<Game>().IsGameOver()) return;

			if (@event.IsActionPressed("pause")) {
				Visible = !Visible;
			}
		}

		public void _BackPressed() {
			GetTree().Paused = false;
			GetTree().Pop();
		}

		public void _Pause() {
			var cd = GetNode<Label>("../Countdown");
			var anim = cd.GetNode<AnimationPlayer>("AnimationPlayer");
			anim.Stop();

			if (!Visible) {
				anim.Play("Countdown");
				cd.Visible = true;
			} else {
				GetTree().Paused = true;
				cd.Visible = false;
			}
		}

		public void _Unpause() {
			GetTree().Paused = false;
			GetNode<Label>("../Countdown").Visible = false;
		}
	}
}
