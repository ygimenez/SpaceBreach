using Godot;
using SpaceBreach.entity.interfaces;
using SpaceBreach.entity.model;
using SpaceBreach.util;

namespace SpaceBreach.entity.enemy {
	public abstract class Invader : Enemy {
		private int _drift;

		protected Invader() : base(hp: 100, speed: 0.25f) {
		}

		public override void _Ready() {
			base._Ready();
			Connect("area_shape_entered", this, nameof(_AreaShapeEntered));
		}

		protected override bool Shoot() {
			var proj = GD.Load<PackedScene>("res://src/entity/projectile/EnemyBullet.tscn");
			var world = Game.GetSafeArea().GetNode<Node2D>("World");

			foreach (Position2D cannon in Cannons) {
				world.AddChild(proj.Instance<Projectile>().With(p => {
					p.Source = this;
					p.GlobalPosition = world.ToLocal(cannon.GlobalPosition);
					p.RotationDegrees = RotationDegrees + 180;
				}));
			}

			return true;
		}

		protected override void Move() {
			var safe = Game.GetSafeArea().GetGlobalRect();
			if (GlobalPosition.x < safe.Position.x) {
				_drift = 1;
			} else if (GlobalPosition.x > safe.End.x) {
				_drift = -1;
			}

			if (_drift != 0) {
				Translate(new Vector2(1, 0) * Speed * Mathf.Sign(_drift));
				_drift -= Mathf.Sign(_drift);
			} else {
				Translate(new Vector2(0, 1) * Speed);
				if (Utils.Rng.Randf() > 0.995) {
					_drift = Utils.Rng.RandiRange(-300, 300);
				}
			}
		}

		public void _AreaShapeEntered(RID _, Area2D entity, int index, int __) {
			var col = entity.GetChild(index).Name;
			if (col == "Left" && _drift < 0 || col == "Right" && _drift > 0) {
				_drift = -_drift;
			}
		}
	}
}
