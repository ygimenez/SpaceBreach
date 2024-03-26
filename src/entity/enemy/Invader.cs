using Godot;
using SpaceBreach.entity.model;
using SpaceBreach.util;

namespace SpaceBreach.entity.enemy {
	public class Invader : Enemy {
		public Invader() : base(100) {

		}

		protected override bool Shoot() {
			var proj = GD.Load<PackedScene>("res://src/entity/projectile/EnemyBullet.tscn");
			var cannon = GetNode<Node2D>("Cannon");
			var world = GetGame().GetNode<Node2D>("SafeArea/World");

			world.AddChild(proj.Instance<Projectile>().With(p => {
				p.Source = this;
				p.GlobalPosition = world.ToLocal(((Position2D) cannon).GlobalPosition);
				p.RotationDegrees = RotationDegrees + 180;
			}));

			return true;
		}

		protected override void Move() {

		}
	}
}
