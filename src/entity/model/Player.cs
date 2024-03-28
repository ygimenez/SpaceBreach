using Godot;
using SpaceBreach.manager;
using SpaceBreach.scene;
using SpaceBreach.util;

namespace SpaceBreach.entity.model {
	public abstract class Player : Entity {
		[Export]
		public float AttackRate;

		[Export]
		public float SpecialRate;

		[Export]
		public float Projectiles;

		public Cooldown AtkCd;
		public Cooldown SpCd;
		private Vector2 _velocity = Vector2.Zero;

		protected Player(uint hp, float attackRate = 1, float specialRate = 1, float projectiles = 1, float speed = 1) : base(hp, speed) {
			AttackRate = attackRate;
			SpecialRate = specialRate;
			Projectiles = projectiles;
		}

		public override void _Ready() {
			base._Ready();
			var game = GetNode<Game>("/root/Control");
			AtkCd = new Cooldown(game, (uint) (200 / AttackRate / ActionSpeed));
			SpCd = new Cooldown(game, (uint) (2000 / SpecialRate / ActionSpeed));

			var contGroup = GetNode("Contrails");
			if (contGroup != null && contGroup.GetChildCount() > 0) {
				var cont = GD.Load<PackedScene>("res://src/entity/particle/Contrail.tscn");

				foreach (var anchor in contGroup.GetChildren()) {
					(anchor as Node)?.AddChild(cont.Instance());
				}
			}
		}

		public override void _Process(float delta) {
			if (Input.IsActionPressed("shoot") && AtkCd.Ready() && Shoot()) {
				AtkCd.Use();
			}

			if (Input.IsActionPressed("special") && SpCd.Ready() && Special()) {
				SpCd.Use();
			}
		}

		public override void _PhysicsProcess(float delta) {
			Accelerate(new Vector2(
				Input.GetAxis("move_right", "move_left"),
				Input.GetAxis("move_down", "move_up")
			).LimitLength() * Speed);

			Translate(_velocity);
		}

		private void Accelerate(Vector2 mov) {
			const float friction = 0.8f;

			var sway = Mathf.Abs(_velocity.x - (_velocity.x + Speed * mov.x) * friction / Engine.TargetFps);
			RotationDegrees = 30 * (sway / (sway + 3)) * 2 * Mathf.Sign(_velocity.x);

			var safe = GetNode<Control>("/root/Control/GameArea/MaxSizeContainer/SafeArea").GetGlobalRect();
			var rect = GetNode<Sprite>("Sprite").GetRect();

			if (!(GlobalPosition - rect.Size / 2 - mov).x.IsBetween(safe.Position.x, safe.Position.x + safe.Size.x - rect.Size.x)) {
				_velocity.x *= -1;
			} else {
				_velocity.x -= (_velocity.x + Speed * mov.x) * friction / Engine.TargetFps;
			}

			if (!(GlobalPosition - rect.Size / 2 - mov).y.IsBetween(safe.Position.y, safe.Position.y + safe.Size.y - rect.Size.y)) {
				_velocity.y *= -1;
			} else {
				_velocity.y -= (_velocity.y + Speed * mov.y) * friction / Engine.TargetFps;
			}
		}

		protected override void OnDamaged(Entity by) {
			base.OnDamaged(by);
			Audio.Cue("res://assets/sounds/player_hit.wav");
		}

		protected override void OnDestroy() {
			base.OnDestroy();
			GetGame().PlayerDeath(this);
			Audio.Cue("res://assets/sounds/player_explode.wav");
		}

		protected abstract bool Shoot();

		protected abstract bool Special();
	}
}
