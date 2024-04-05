using System.Collections.Generic;
using System.Net.Http;
using System.Text;
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

		private float _origTimeScale;

		public override void _Ready() {
			GetNode<Button>("Back").Connect("pressed", this, nameof(_BackPressed));
			GetNode<Button>("Leaderboard/Submit").Connect("pressed", this, nameof(_SubmitPressed));
			GetNode<LineEdit>("Leaderboard/LBName").With(edit => {
				edit.Connect("text_changed", this, nameof(_MakeUppercase), edit);
			});

			Audio.AttachUiAudio(this);
		}

		public override void _Process(float delta) {
			if (!Global.Mobile) {
				Input.MouseMode = Visible ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured;
			}

			var game = GetNode<Game>("/root/Control");
			GetNode<Label>("Label").Text = GetParent<Game>().IsGameOver() ? $"GAME OVER\nSCORE: {game.Score}" : "PAUSED";
		}

		public override void _Input(InputEvent @event) {
			if (GetParent<Game>().IsGameOver()) return;

			if (@event.IsActionPressed("pause")) {
				Visible = !Visible;
			}
		}

		public void _BackPressed() {
			GetTree().Paused = false;
			GetTree().PopUntil(_ => false);
		}

		public void _MakeUppercase(string text, LineEdit edit) {
			var carPos = edit.CaretPosition;
			edit.Text = text.ToUpper();
			edit.CaretPosition = carPos;
		}

		public void _SubmitPressed() {
			var game = GetNode<Game>("/root/Control");
			var name = GetNode<LineEdit>("Leaderboard/LBName");
			if (!name.Text.Empty()) {
				Global.Instance.HttpPost("https://api.shirojbot.site/v2/sbreach/leaderboard",
					JSON.Print(new Dictionary<string, object> {
						{ "initials", name.Text },
						{ "score", game.Score }
					})
				);
			}

			GetNode<Control>("Leaderboard").Visible = false;
			GetNode<Control>("Back").Visible = true;
		}

		public void _Pause() {
			var cd = GetNode<Label>("../Countdown");
			var anim = cd.GetNode<AnimationPlayer>("AnimationPlayer");
			anim.Stop();

			if (!Visible) {
				anim.Play("Countdown");
				cd.Visible = true;
			} else {
				_origTimeScale = Engine.TimeScale;
				Engine.TimeScale = 1;
				GetTree().Paused = true;
				cd.Visible = false;
			}
		}

		public void _Unpause() {
			Engine.TimeScale = _origTimeScale;
			_origTimeScale = 0;
			GetTree().Paused = false;
			GetNode<Label>("../Countdown").Visible = false;
		}
	}
}
