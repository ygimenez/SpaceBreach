﻿using Godot;
using SpaceBreach.entity.model;
using SpaceBreach.manager;
using SpaceBreach.util;

namespace SpaceBreach.entity.player {
	public abstract class Fighter : Player {
		protected Fighter() : base(hp: 200, speed: 0.6f) {
		}

		protected override bool Shoot() {
			var proj = GD.Load<PackedScene>("res://src/entity/projectile/PlayerBullet.tscn");
			var world = GetGame().GetSafeArea().GetNode<Node2D>("World");

			foreach (Position2D cannon in Cannons) {
				for (var i = 0; i < Projectiles; i++) {
					var offset = 30 / (Projectiles + 1) * (i + 1);

					world.AddChild(proj.Instance<Projectile>().With(p => {
						p.Source = this;
						p.GlobalPosition = world.ToLocal(cannon.GlobalPosition);
						p.RotationDegrees = RotationDegrees + (15 - offset);
						p.Damage = (uint) (p.Damage / Projectiles + p.Damage * 0.25f * (Projectiles - 1));
					}));
				}
			}

			Audio.Cue("res://assets/sounds/player_shoot.wav");
			return true;
		}

		protected override bool Special() {
			var proj = GD.Load<PackedScene>("res://src/entity/projectile/PlayerBomb.tscn");
			var world = GetGame().GetSafeArea().GetNode<Node2D>("World");

			foreach (Position2D cannon in Cannons) {
				for (var i = 0; i < Projectiles; i++) {
					world.AddChild(proj.Instance<Projectile>().With(p => {
						p.Source = this;
						p.GlobalPosition = world.ToLocal(cannon.GlobalPosition);
						p.RotationDegrees = RotationDegrees;
					}));
				}
			}

			Audio.Cue("res://assets/sounds/player_bomb.wav");
			return true;
		}
	}
}
