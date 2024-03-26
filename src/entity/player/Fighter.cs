using Godot;
using SpaceBreach.entity.model;
using SpaceBreach.manager;
using SpaceBreach.util;

namespace SpaceBreach.entity.player {
	public class Fighter : Player {
		public Fighter() : base(200) {
		}

		protected override bool Shoot() {
			var cannons = GetNode("Cannons");
			if (cannons != null && cannons.GetChildCount() > 0) {
				var proj = GD.Load<PackedScene>("res://src/entity/projectile/PlayerBullet.tscn");
				var world = GetGame().GetNode<Node2D>("SafeArea/World");

				foreach (var cannon in cannons.GetChildren()) {
					for (var i = 0; i < Projectiles; i++) {
						world.AddChild(proj.Instance<Projectile>().With(p => {
							p.Source = this;
							p.GlobalPosition = world.ToLocal(((Position2D) cannon).GlobalPosition);
							p.RotationDegrees = RotationDegrees;
						}));
					}
				}

				Audio.Cue(this, "res://assets/sounds/player_shoot.wav");
			}

			return true;
		}

		protected override bool Special() {
			var cannons = GetNode("Cannons");
			if (cannons != null && cannons.GetChildCount() > 0) {
				var proj = GD.Load<PackedScene>("res://src/entity/projectile/PlayerBomb.tscn");
				var world = GetGame().GetNode<Node2D>("SafeArea/World");

				foreach (var cannon in cannons.GetChildren()) {
					for (var i = 0; i < Projectiles; i++) {
						world.AddChild(proj.Instance<Projectile>().With(p => {
							p.Source = this;
							p.GlobalPosition = world.ToLocal(((Position2D) cannon).GlobalPosition);
							p.RotationDegrees = RotationDegrees;
						}));
					}
				}
			}

			return true;
		}
	}
}
