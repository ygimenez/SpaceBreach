using System.Linq;
using Godot;
using SpaceBreach.entity.model;
using SpaceBreach.entity.projectile;
using SpaceBreach.util;

namespace SpaceBreach.entity.player {
	public abstract class Carrier : Player {
		public bool Defending;
		public uint DroneAngle;

		protected Carrier() : base(hp: 300, attackRate: 1.5f, speed: 0.45f, specialRate: 0, projectiles: 3, damageMult: 1.25f) {
		}

		public override void _Process(float delta) {
			base._Process(delta);
			DroneAngle += (uint) (Defending ? 1.5 : 3);

			var drones = GetChildren().OfType<PlayerDrone>().ToList();
			if (drones.Count < Projectiles) {
				for (var i = 0; i < Projectiles - drones.Count; i++) {
					AddChild(Projectile.Poll<PlayerDrone>().With(p => p.Source = this));
				}
			}

			foreach (var drone in drones.Where(d => d.Cooldown > 0)) {
				drone.Cooldown--;
			}
		}

		protected override bool Shoot() {
			if (Defending) return false;

			var world = Game.GetSafeArea().GetNode<Node2D>("World");
			var drones = GetChildren().OfType<PlayerDrone>().ToList();
			foreach (var drone in drones) {
				var pos = drone.GlobalPosition;

				RemoveChild(drone);
				world.AddChild(drone.With(d => {
					d.Speed = 2;
					d.Position = world.ToLocal(pos);
				}));
			}

			return true;
		}

		protected override bool Special() {
			Defending = !Defending;
			return true;
		}
	}
}
