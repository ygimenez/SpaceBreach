using Godot;

namespace SpaceBreach.component {
	public abstract class Controller : Control {
		private SceneTreeTimer _dtTimer;
		private bool _pressed, _mustRelease;

		public override void _Input(InputEvent @event) {
			if (@event is InputEventScreenTouch) {
				_pressed = @event.IsPressed();

				if (_pressed) {
					if (_dtTimer != null) {
						_dtTimer = null;
						Input.ActionPress("special");
						_mustRelease = true;
						return;
					}

					_dtTimer = GetTree().CreateTimer(0.15f);
					_dtTimer.Connect("timeout", this, nameof(_SingleTap));
				} else {
					Input.ActionRelease("shoot");
					Input.ActionRelease("special");
					_mustRelease = false;
				}
			}
		}

		public override void _Process(float delta) {
			if (Input.IsActionPressed("shoot") && !_pressed && _mustRelease) {
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
