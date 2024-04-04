using Godot;
using SpaceBreach.entity.model;
using SpaceBreach.entity.projectile;
using SpaceBreach.manager;
using SpaceBreach.util;

namespace SpaceBreach.entity.player {
	public abstract class Fighter : Player {
		protected Fighter() : base(hp: 200, speed: 0.6f) {
		}

		protected override bool Shoot() {
			var world = Game.GetSafeArea().GetNode<Node2D>("World");

			foreach (Position2D cannon in Cannons) {
				for (var i = 0; i < Projectiles; i++) {
					var offset = 30 / (Projectiles + 1) * (i + 1);

					world.AddChild(Projectile.Poll<PlayerBullet>().With(p => {
						p.Source = this;
						p.GlobalPosition = world.ToLocal(cannon.GlobalPosition);
						p.RotationDegrees = RotationDegrees + (15 - offset);
						p.Damage = (uint) (p.Damage / Projectiles + p.Damage * 0.25f * (Projectiles - 1) * DamageMult);
					}));
				}
			}

			Audio.Cue("res://assets/sounds/player_shoot.wav");
			return true;
		}

		protected override bool Special() {
			var world = Game.GetSafeArea().GetNode<Node2D>("World");

			foreach (Position2D cannon in Cannons) {
				for (var i = 0; i < Projectiles; i++) {
					world.AddChild(Projectile.Poll<PlayerBomb>().With(p => {
						p.Source = this;
						p.GlobalPosition = world.ToLocal(cannon.GlobalPosition);
						p.RotationDegrees = RotationDegrees;
						p.Damage = (uint) (p.Damage * DamageMult);
					}));
				}
			}

			Audio.Cue("res://assets/sounds/player_bomb.wav");
			return true;
		}
	}
}
