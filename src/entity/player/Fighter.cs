using Godot;
using SpaceBreach.entity.model;
using SpaceBreach.util;

namespace SpaceBreach.entity.player {
	public class Fighter : Player {
		public Fighter() : base(200) {
		}

		protected override bool Shoot() {
			var cannons = GetNode("Cannons");
			if (cannons != null && cannons.GetChildCount() > 0) {
				var proj = GD.Load<PackedScene>("res://src/entity/projectile/PlayerBullet.tscn");

				foreach (var cannon in cannons.GetChildren()) {
					for (var i = 0; i < Projectiles; i++) {
						GetParent().AddChild(proj.Instance().With(p => {
							((Area2D) p).GlobalPosition = GetGame().GetSafeArea().ToLocal(((Position2D) cannon).GlobalPosition);
							((Area2D) p).RotationDegrees = RotationDegrees;
						}));
					}
				}
			}

            return true;
		}

		protected override bool Special() {
			var cannons = GetNode("Cannons");
			if (cannons != null && cannons.GetChildCount() > 0) {
				var proj = GD.Load<PackedScene>("res://src/entity/projectile/PlayerBullet.tscn");

				foreach (var cannon in cannons.GetChildren()) {
					for (var i = 0; i < Projectiles; i++) {
						GetParent().AddChild(proj.Instance().With(p => {
							((Area2D) p).GlobalPosition = GetParent().ToLocal(((Position2D) cannon).GlobalPosition);
							((Area2D) p).RotationDegrees = RotationDegrees;
						}));
					}
				}
			}

            return true;
		}
	}
}
