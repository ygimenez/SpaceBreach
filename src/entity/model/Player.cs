using Godot;
using SpaceBreach.scene;
using SpaceBreach.util;

namespace SpaceBreach.entity.model {
	public class Player : Entity {
		[Export]
		public float Speed = 1;

		[Export]
		public Player Self;

		[Export]
		public Game Game;

		private Cooldown _atkCd;
		private Vector2 _velocity = Vector2.Zero;

		public Player() {
			Self = this;
		}

		public override void _Ready() {
			base._Ready();
			_atkCd = new Cooldown(Game);

			var contGroup = GetNode("Contrails");
			if (contGroup != null && contGroup.GetChildCount() > 0) {
				var cont = GD.Load<PackedScene>("res://src/entity/particle/Contrail.tscn");

				foreach (var anchor in contGroup.GetChildren()) {
					((Node) anchor).AddChild(cont.Instance().With(n => {
						n.Set("Ship", this);
					}));
				}
			}
		}

		public override void _Process(float delta) {
			if (Input.IsActionPressed("shoot") && _atkCd.Use()) {

			}
		}

		public override void _PhysicsProcess(float delta) {
			Accelerate(Input.GetVector("move_right", "move_left", "move_down", "move_up"));
			Translate(_velocity);
		}

		private void Accelerate(Vector2 mov) {
			const float friction = 0.4f;

			var sway = (_velocity.x - (_velocity.x + Speed * mov.x) * friction / Engine.GetFramesPerSecond()).Clamp(-1, 1);
			RotationDegrees = 30 * sway;

			var safe = GetNode<Control>("/root/Control/SafeArea").GetGlobalRect();
			var rect = GetNode<Sprite>("Sprite").GetRect();

			if (!(GlobalPosition - rect.Size / 2 - mov).x.IsBetween(safe.Position.x, safe.Position.x + safe.Size.x - rect.Size.x)) {
				_velocity.x *= -1;
			} else {
				_velocity.x -= (_velocity.x + Speed * mov.x) * friction / Engine.GetFramesPerSecond();
			}

			if (!(GlobalPosition - rect.Size / 2 - mov).y.IsBetween(safe.Position.y, safe.Position.y + safe.Size.y - rect.Size.y)) {
				_velocity.y *= -1;
			} else {
				_velocity.y -= (_velocity.y + Speed * mov.y) * friction / Engine.GetFramesPerSecond();
			}
		}
	}
}
