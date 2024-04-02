using Godot;
using SpaceBreach.entity.interfaces;
using SpaceBreach.entity.model;
using SpaceBreach.util;

namespace SpaceBreach.entity.enemy {
	public abstract class Defender : Enemy, ICannotSpawn {
		private int _index, _radius;
		private int _tgtRadius = 90;
		private float _posFac = 1;
		private bool _expanded;

		private Vector2 _tgtOrbit;

		private Vector2 PreviousOrbit { get; set; }

		private Vector2 TargetOrbit {
			get => _tgtOrbit;
			set {
				if (value != _tgtOrbit) {
					PreviousOrbit = _tgtOrbit;
				}

				_tgtOrbit = value;
			}
		}

		protected Defender() : base(hp: 500, attackRate: 1.5f) {
		}

		public override void _Ready() {
			base._Ready();

			foreach (var child in GetParent().GetChildren()) {
				if (child is Defender) {
					_index++;
				}
			}
		}

		protected override void Move() {
			var parent = this.FindParent<Mothership>();
			var safe = GetGame().GetSafeArea().GetGlobalRect();

			if (parent.Enraged) {
				if (_posFac >= 1 && !_expanded) {
					TargetOrbit = safe.GetCenter();
					RawSpeed /= 4;
					_posFac = 0;
					_tgtRadius = (int) (Mathf.Sqrt(safe.Size.x * safe.Size.x + safe.Size.y * safe.Size.y) / 2);
					_expanded = true;
				}
			} else {
				TargetOrbit = GetParent<Node2D>().GlobalPosition;
			}

			int toRadius;
			if (_posFac < 1) {
				toRadius = 0;
				_posFac = Mathf.Clamp(_posFac + 0.005f, 0, 1);
			} else {
				toRadius = _tgtRadius;
			}

			GlobalPosition = PreviousOrbit.LinearInterpolate(TargetOrbit, _posFac) + new Vector2(
				Mathf.Sin(Mathf.Deg2Rad(parent.DefAngle * Speed + 36 * _index)) * _radius,
				Mathf.Cos(Mathf.Deg2Rad(parent.DefAngle * Speed + 36 * _index)) * _radius
			);

			if (_expanded && _posFac > 0.5) {
				GlobalPosition = GlobalPosition.Clamp(safe.Position, safe.End);
			}

			if (_radius != toRadius) {
				_radius += Mathf.Sign(toRadius - _radius);
			}
		}

		protected override bool Shoot() {
			var parent = this.FindParent<Mothership>();
			if (parent.Enraged && _posFac >= 1 && _radius == _tgtRadius && Utils.Rng.Randf() > 0.9f) {
				var proj = GD.Load<PackedScene>("res://src/entity/projectile/EnemyBullet.tscn");
				var world = GetGame().GetSafeArea().GetNode<Node2D>("World");
				var game = GetGame();

				foreach (Position2D cannon in Cannons) {
					world.AddChild(proj.Instance<Projectile>().With(p => {
						p.Source = this;
						p.GlobalPosition = world.ToLocal(cannon.GlobalPosition);
						p.Rotation = game.Player.GlobalPosition.AngleToPoint(cannon.GlobalPosition) + Mathf.Deg2Rad(90);
					}));
				}

				return true;
			}

			return false;
		}

		protected override void OnDamaged(Entity by, long value) {
			base.OnDamaged(by, value);
			this.FindParent<Mothership>().AddHp(by, value / 3);
		}

		public override void _AreaEntered(Area2D entity) {
			if (_radius != _tgtRadius) return;
			base._AreaEntered(entity);
		}
	}
}
