using Godot;
using SpaceBreach.entity.model;
using SpaceBreach.manager;
using SpaceBreach.util;

namespace SpaceBreach.entity.player {
	public abstract class Fighter : Player {
		protected Fighter() : base(hp: 200, speed: 0.8f) {
		}

		protected override bool Shoot() {
			var cannons = GetNode("Cannons");
			if (cannons != null && cannons.GetChildCount() > 0) {
				var proj = GD.Load<PackedScene>("res://src/entity/projectile/PlayerBullet.tscn");
				var world = GetGame().GetSafeArea().GetNode<Node2D>("World");

				foreach (Position2D cannon in cannons.GetChildren()) {
					for (var i = 0; i < Projectiles; i++) {
						var offset = 30 / (Projectiles + 1) * (i + 1);

						world.AddChild(proj.Instance<Projectile>().With(p => {
							p.Source = this;
							p.GlobalPosition = world.ToLocal(cannon.GlobalPosition);
							p.RotationDegrees = RotationDegrees + (15 - offset);
							p.Damage -= (uint) ((Projectiles - 1) * p.Damage / 4);
						}));
					}
				}

				Audio.Cue("res://assets/sounds/player_shoot.wav");
			}

			return true;
		}

		protected override bool Special() {
			var cannons = GetNode("Cannons");
			if (cannons != null && cannons.GetChildCount() > 0) {
				var proj = GD.Load<PackedScene>("res://src/entity/projectile/PlayerBomb.tscn");
				var world = GetGame().GetSafeArea().GetNode<Node2D>("World");

				foreach (Position2D cannon in cannons.GetChildren()) {
					for (var i = 0; i < Projectiles; i++) {
						world.AddChild(proj.Instance<Projectile>().With(p => {
							p.Source = this;
							p.GlobalPosition = world.ToLocal(cannon.GlobalPosition);
							p.RotationDegrees = RotationDegrees;
						}));
					}
				}

				Audio.Cue("res://assets/sounds/player_bomb.wav");
			}

			return true;
		}
	}
}
