using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using SpaceBreach.entity.interfaces;
using SpaceBreach.manager;
using SpaceBreach.scene;
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

		protected Enemy(uint hp, float attackRate = 1, float speed = 1) : base(hp, speed) {
			AttackRate = attackRate;

			_drop = Utils.Rng.Randfn() > 0.9;
			if (_drop) {
				Modulate = Colors.Yellow;
			}
		}

		public override void _Ready() {
			base._Ready();
			var game = GetNode<Game>("/root/Control");

			BaseHp = Hp = (uint) (BaseHp * game.Level * (_drop ? 1.5f : 1));
			Cooldown = new Cooldown(game, (uint) (500 / (AttackRate + 0.2f * game.Level)));
		}

		public override void _Process(float delta) {
			if (Visible && Cooldown.Ready() && Shoot()) {
				Cooldown.Use();
				Audio.Cue("res://assets/sounds/enemy_fire.wav");
			}
		}

		public override void _PhysicsProcess(float delta) {
			Move();
		}

		public uint GetCost() {
			return (uint) (BaseHp * AttackRate);
		}

		protected abstract bool Shoot();

		protected abstract void Move();

		protected override void OnDamaged(Entity by) {
			base.OnDamaged(by);
			Audio.Cue("res://assets/sounds/enemy_hit.wav");
		}

		protected override void OnDestroy() {
			base.OnDestroy();
			GetGame().Score += GetCost();
			if (_drop) {
				var world = GetGame().GetSafeArea().GetNode<Node2D>("World");

				world.AddChild(Utils.Load(_drops.Random()).Instance<Pickup>().With(p => {
					p.GlobalPosition = world.ToLocal(GlobalPosition);
				}));
			}

			if (this is IBoss) {
				GetGame().Boss = null;
			}
		}
	}
}
