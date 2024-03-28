using Godot;

namespace SpaceBreach.component {
	public abstract class Controller : Button {
		private SceneTreeTimer _dtTimer;
		private bool _mustRelease;

		public override void _Ready() {
			Connect("button_down", this, nameof(_ButtonDown));
			Connect("button_up", this, nameof(_ButtonUp));
		}

		public void _ButtonDown() {
			if (_dtTimer != null) {
				_dtTimer = null;
				Input.ActionPress("special");
				_mustRelease = true;
				return;
			}

			_dtTimer = GetTree().CreateTimer(0.15f);
			_dtTimer.Connect("timeout", this, nameof(_SingleTap));
		}

		public void _ButtonUp() {
			Input.ActionRelease("shoot");
			Input.ActionRelease("special");
			_mustRelease = false;
		}

		public override void _Process(float delta) {
			if (Input.IsActionPressed("shoot") && !Pressed && _mustRelease) {
				Input.ActionRelease("shoot");
				_mustRelease = false;
			}
		}

		public void _SingleTap() {
			if (_dtTimer == null) return;

			_dtTimer = null;
			Input.ActionPress("shoot");
			_mustRelease = true;
		}
	}
}
