using System.Threading.Tasks;
using Godot;
using SpaceBreach.manager;
using SpaceBreach.util;

namespace SpaceBreach.entity.model {
	public abstract class Player : Entity {
		[Export]
		public float AttackRate;

		[Export]
		public float SpecialRate;

		[Export]
		public float Projectiles;

		[Export]
		public float DamageMult;

		public Cooldown AtkCd;
		public CostCooldown SpCd;
		private Vector2 _velocity = Vector2.Zero;
		private uint _iframes;

		protected Player(uint hp, float attackRate = 1, float specialRate = 1, float projectiles = 1, float damageMult = 1, float speed = 1) : base(hp, speed) {
			AttackRate = attackRate;
			SpecialRate = specialRate;
			Projectiles = projectiles;
			DamageMult = damageMult;
		}

		public override void _Ready() {
			var game = Game;
			AtkCd = new Cooldown(game, this, 200);
			SpCd = new CostCooldown(game, this, 2000);

			var contGroup = GetNode("Contrails");
			if (contGroup != null && contGroup.GetChildCount() > 0) {
				var cont = GD.Load<PackedScene>("res://src/entity/particle/Contrail.tscn");

				foreach (var anchor in contGroup.GetChildren()) {
					(anchor as Node)?.AddChild(cont.Instance());
				}
			}

			base._Ready();
		}

		public override void _Process(float delta) {
			AtkCd.Time = (uint) Mathf.Max(1, 200 / AttackRate);
			if (_iframes > 0 && Utils.IsBetween(_iframes % 50, 0, 24)) {
				Modulate = Colors.Transparent;
			} else {
				Modulate = Colors.White;
			}

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
			if (Engine.TimeScale < 1 && Game.Tick % 20 == 0) {
				var world = Game.GetSafeArea().GetNode<Node2D>("World");
				Utils.AddGhost(world, GetNode<Node2D>("Sprite"), 0.5f);
			}

			if (_iframes > 0) {
				_iframes--;
			}
		}

		private void Accelerate(Vector2 mov) {
			const float friction = 0.8f;

			var sway = Mathf.Abs(_velocity.x - (_velocity.x + Speed * mov.x) * friction / Engine.TargetFps);
			RotationDegrees = 30 * (sway / (sway + 3)) * 2 * Mathf.Sign(_velocity.x);

			var safe = GetNode<Control>("/root/Control/GameArea/MaxSizeContainer/SafeArea").GetGlobalRect();

			if (!(GlobalPosition - Size / 2 - mov).x.IsBetween(safe.Position.x, safe.Position.x + safe.Size.x - Size.x)) {
				_velocity.x *= -1;
			} else {
				_velocity.x -= (_velocity.x + Speed * mov.x) * friction / Engine.TargetFps;
			}

			if (!(GlobalPosition - Size / 2 - mov).y.IsBetween(safe.Position.y, safe.Position.y + safe.Size.y - Size.y)) {
				_velocity.y *= -1;
			} else {
				_velocity.y -= (_velocity.y + Speed * mov.y) * friction / Engine.TargetFps;
			}
		}

		public override void AddHp(Entity source, long value) {
			if (value < 0 && _iframes > 0) return;

			base.AddHp(source, value);
		}

		protected override void OnDamaged(Entity by, long value) {
			_iframes = 200;
			Game.Streak = 0;
			Audio.Cue("res://assets/sounds/player_hit.wav");
		}

		protected override Task OnDestroy() {
			base.OnDestroy();
			Game.PlayerDeath(this);
			GetParent().AddChild(GD.Load<PackedScene>("res://src/entity/misc/PlayerDeath.tscn").Instance<Node2D>().With(d =>
				d.Position = Position
			));

			Audio.Cue("res://assets/sounds/player_explode.wav");
			return Task.CompletedTask;
		}

		protected abstract bool Shoot();

		protected abstract bool Special();
	}
}
