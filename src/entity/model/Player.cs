using Godot;
using SpaceBreach.manager;
using SpaceBreach.scene;
using SpaceBreach.util;

namespace SpaceBreach.entity.model {
	public abstract class Player : Entity {
		[Export]
		public float SpeedMult;

		[Export]
		public float AttackRate;

		[Export]
		public float SpecialRate;

		[Export]
		public float Projectiles;

		public Cooldown AtkCd;
		public Cooldown SpCd;
		private Vector2 _velocity = Vector2.Zero;

		protected Player(uint hp, float speedMult = 1, float attackRate = 1, float specialRate = 1, float projectiles = 1, float speed = 1) : base(hp, speed) {
			SpeedMult = speedMult;
			AttackRate = attackRate;
			SpecialRate = specialRate;
			Projectiles = projectiles;
		}

		public override void _Ready() {
			base._Ready();
			var game = GetNode<Game>("/root/Control");
			AtkCd = new Cooldown(game, (uint) (200 / AttackRate));
			SpCd = new Cooldown(game, (uint) (1000 / SpecialRate));

			var contGroup = GetNode("Contrails");
			if (contGroup != null && contGroup.GetChildCount() > 0) {
				var cont = GD.Load<PackedScene>("res://src/entity/particle/Contrail.tscn");

				foreach (var anchor in contGroup.GetChildren()) {
					(anchor as Node)?.AddChild(cont.Instance());
				}
			}
		}

		public override void _UnhandledInput(InputEvent @event) {
			if (@event.IsActionPressed("shoot") && AtkCd.Ready() && Shoot()) {
				AtkCd.Use();
			}

			if (@event.IsActionPressed("special") && SpCd.Ready() && Special()) {
				SpCd.Use();
			}
		}

		public override void _PhysicsProcess(float delta) {
			Accelerate(Input.GetVector("move_right", "move_left", "move_down", "move_up") * (Speed * SpeedMult));
			Translate(_velocity);
		}

		private void Accelerate(Vector2 mov) {
			const float friction = 0.4f;

			var sway = (_velocity.x - (_velocity.x + Speed * SpeedMult * mov.x) * friction / Engine.GetFramesPerSecond()).Clamp(-1, 1);
			RotationDegrees = 30 * sway;

			var safe = GetNode<Control>("/root/Control/GameArea/SafeArea").GetGlobalRect();
			var rect = GetNode<Sprite>("Sprite").GetRect();

			if (!(GlobalPosition - rect.Size / 2 - mov).x.IsBetween(safe.Position.x, safe.Position.x + safe.Size.x - rect.Size.x)) {
				_velocity.x *= -1;
			} else {
				_velocity.x -= (_velocity.x + Speed * SpeedMult * mov.x) * friction / Engine.GetFramesPerSecond();
			}

			if (!(GlobalPosition - rect.Size / 2 - mov).y.IsBetween(safe.Position.y, safe.Position.y + safe.Size.y - rect.Size.y)) {
				_velocity.y *= -1;
			} else {
				_velocity.y -= (_velocity.y + Speed * SpeedMult * mov.y) * friction / Engine.GetFramesPerSecond();
			}
		}

		protected override void OnDamaged(Entity by) {
			Audio.Cue(this, "res://assets/sounds/hit.wav");
		}

		protected abstract bool Shoot();

		protected abstract bool Special();
	}
}
