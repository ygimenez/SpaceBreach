using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using SpaceBreach.entity.interfaces;
using SpaceBreach.manager;
using SpaceBreach.util;

namespace SpaceBreach.entity.model {
	public abstract class Enemy : Entity, ITracked {
		private readonly bool _drop;

		private readonly List<Type> _drops = typeof(Pickup).Assembly
			.GetTypes()
			.Where(t => t.IsSubclassOf(typeof(Pickup)))
			.ToList();

		[Export]
		public float AttackRate;

		protected Cooldown Cooldown;
		private bool _tweened;
		private bool _enraged;

		protected Enemy(uint hp, float attackRate = 1, float speed = 1) : base(hp, speed) {
			AttackRate = attackRate;

			_drop = Utils.Rng.Randfn() > 0.9;
			if (_drop) {
				GetNode<Node2D>("Sprite").SelfModulate = Colors.Yellow;
			}
		}

		public override void _Ready() {
			var game = Game;

			ActionSpeed = 1 + 0.2f * (game.Level - 1);
			BaseHp = Hp = (uint) (BaseHp * game.Level * (_drop ? 1.5f : 1));
			Cooldown = new Cooldown(game, this, 500);

			Connect("area_entered", this, nameof(_AreaEntered));
			base._Ready();
		}

		public override void _Process(float delta) {
			Cooldown.Time = (uint) Mathf.Max(1, 500 / AttackRate);

			if (Visible && Cooldown.Ready() && Shoot()) {
				Cooldown.Use();
				Audio.Cue("res://assets/sounds/enemy_fire.wav");
			}

			if (this is IBoss b && b.Enraged && !_tweened) {
				_tweened = true;

				var tween = CreateTween();
				tween.TweenProperty(this, "modulate", Colors.Red, 0.5f);
			}
		}

		public override void _PhysicsProcess(float delta) {
			Move();

			if (this is IBoss b && b.Enraged && !_enraged) {
				_enraged = true;
				OnEnrage();
			}
		}

		public uint GetCost() {
			return (uint) (BaseHp * AttackRate);
		}

		protected abstract bool Shoot();

		protected abstract void Move();

		protected override void OnDamaged(Entity by, long value) {
			Audio.Cue("res://assets/sounds/enemy_hit.wav");
		}

		public override void _ExitTree() {
			if (!(this is ICannotSpawn)) {
				Game.SpawnPool++;
			}

			if (this is IBoss) {
				Game.Boss = null;
			}
		}

		public virtual void _AreaEntered(Area2D entity) {
			if (entity is Player p) {
				p.AddHp(this, -Hp);
				AddHp(this, -p.GetHp());
			}
		}

		protected virtual void OnEnrage() {
			ActionSpeed *= 1.5f;
			var world = Game.GetSafeArea().GetNode<Node2D>("World");

			world.AddChild(GD.Load<PackedScene>("res://src/entity/pickup/LargeHpPickup.tscn").Instance<Pickup>().With(p => {
				p.Position = Position;
			}));
		}

		protected override Task OnDestroy() {
			base.OnDestroy();
			var game = Game;
			game.Score += (uint) ((this is IBoss ? BaseHp : GetCost()) * (1 + game.Streak / 10f) * (this is IBoss ? 5 : 1));
			game.Streak++;
			game.Player.SpCd.Credits += (uint) Mathf.Abs(BaseHp * game.Player.SpecialRate);

			if (_drop) {
				var world = Game.GetSafeArea().GetNode<Node2D>("World");

				world.AddChild(Utils.Load(_drops.Random()).Instance<Pickup>().With(p => {
					p.Position = Position;
				}));
			}

			Audio.Cue("res://assets/sounds/enemy_explode.wav");
			return Task.CompletedTask;
		}
	}
}
