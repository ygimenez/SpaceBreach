using Godot;
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
			var cannon = GetNode<Node2D>("Cannon");
			var world = GetGame().GetSafeArea().GetNode<Node2D>("World");

			world.AddChild(proj.Instance<Projectile>().With(p => {
				p.Source = this;
				p.GlobalPosition = world.ToLocal(cannon.GlobalPosition);
				p.RotationDegrees = RotationDegrees + 180;
			}));

			return true;
		}

		protected override void Move() {
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
			if (entity.GetChild(index).Name.EqualsAny("Left", "Right")) {
				_drift = -_drift;
			}
		}
	}
}
