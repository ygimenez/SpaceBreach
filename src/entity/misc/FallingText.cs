using Godot;
using SpaceBreach.scene;
using SpaceBreach.util;

namespace SpaceBreach.entity.misc {
	public abstract class FallingText : Node2D {
		[Export]
		public string Text = "";

		[Export]
		public Vector2 Origin;

		private bool _released;

		public override void _Ready() {
			GetNode<Label>("Label").Text = Text;
			GetNode<Game>("/root/Control").TextLeft++;
		}

		public override void _PhysicsProcess(float delta) {
			Translate(Vector2.Down * 0.8f);
			GetNode<Label>("Label").With(l => {
				l.RectPosition = l.GetFont("font").GetStringSize(Text) * -Origin;
			});

			var reference = GetNode<Game>("/root/Control");
			if (!_released && GlobalPosition.y > ToLocal(reference.RectSize).y / 2) {
				GetNode<Game>("/root/Control").TextLeft--;
				GetNode<AnimationPlayer>("AnimationPlayer").Play("Fade");
				_released = true;
			}
		}
	}
}
