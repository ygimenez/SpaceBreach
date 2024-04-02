﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using SpaceBreach.entity.interfaces;
using SpaceBreach.manager;
using SpaceBreach.util;

namespace SpaceBreach.entity.model {
	public abstract class Enemy : Entity {
		private readonly bool _drop;
		private bool _tweened;

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
				SelfModulate = Colors.Yellow;
			}
		}

		public override void _Ready() {
			base._Ready();
			var game = GetGame();

			ActionSpeed = 1 + 0.2f * (game.Level - 1);
			BaseHp = Hp = (uint) (BaseHp * game.Level * (_drop ? 1.5f : 1));
			Cooldown = new Cooldown(game, (uint) (500 / AttackRate));

			Connect("area_entered", this, nameof(_AreaEntered));
		}

		public override void _Process(float delta) {
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
		}

		public uint GetCost() {
			return (uint) (BaseHp * AttackRate);
		}

		protected abstract bool Shoot();

		protected abstract void Move();

		protected override void OnDamaged(Entity by, long value) {
			if (by is Player p) {
				p.SpCd.Credits += (uint) Mathf.Abs(value);
			}

			Audio.Cue("res://assets/sounds/enemy_hit.wav");
		}

		public override void _ExitTree() {
			GetGame().SpawnPool++;
			if (this is IBoss) {
				GetGame().Boss = null;
			}
		}

		public virtual void _AreaEntered(Area2D entity) {
			if (entity is Player p) {
				p.AddHp(this, -Hp);
				AddHp(this, -p.GetHp());
			}
		}

		protected override Task OnDestroy() {
			base.OnDestroy();
			var game = GetGame();
			game.Score += (uint) (GetCost() * (1 + game.Streak / 10f) * (this is IBoss ? 5 : 1));
			game.Streak++;

			if (_drop) {
				var world = GetGame().GetSafeArea().GetNode<Node2D>("World");

				world.AddChild(Utils.Load(_drops.Random()).Instance<Pickup>().With(p => {
					p.Position = Position;
				}));
			}

			Audio.Cue("res://assets/sounds/enemy_explode.wav");
			return Task.CompletedTask;
		}
	}
}
