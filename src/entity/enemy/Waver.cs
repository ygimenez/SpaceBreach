using Godot;
using SpaceBreach.entity.interfaces;
using SpaceBreach.entity.model;
using SpaceBreach.entity.projectile;
using SpaceBreach.util;

namespace SpaceBreach.entity.enemy {
	public abstract class Waver : Enemy {
		private int _angle;

		protected Waver() : base(hp: 125, 0.75f, speed: 0.35f) {
		}

		protected override bool Shoot() {
			var world = Game.GetSafeArea().GetNode<Node2D>("World");

			var shot = 0;
			foreach (Position2D cannon in Cannons) {
				world.AddChild(Projectile.Poll<EnemyWavingOrb>().With(p => {
					p.Source = this;
					p.GlobalPosition = world.ToLocal(cannon.GlobalPosition);
					p.RotationDegrees = 180 + 15 - 10 * shot++;
				}));
			}

			return true;
		}

		protected override void Move() {
			var wave = Utils.FSin(_angle++) / 2;
			Translate(new Vector2(wave, 1 - Mathf.Abs(wave)) * Speed);
		}
	}
}
