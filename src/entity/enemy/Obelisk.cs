using Godot;
using SpaceBreach.entity.model;
using SpaceBreach.entity.projectile;
using SpaceBreach.util;

namespace SpaceBreach.entity.enemy {
	public abstract class Obelisk : Enemy {
		private Vector2 _prevSpot, _tgtSpot;
		private float _fac;
		private uint _timeLeft;

		protected Obelisk() : base(hp: 200, speed: 0.005f) {
		}

		public override void _Ready() {
			base._Ready();
			_prevSpot = Position;
		}

		protected override bool Shoot() {
			if (_fac >= 1) {
				var world = Game.GetSafeArea().GetNode<Node2D>("World");

				foreach (Position2D cannon in Cannons) {
					world.AddChild(Projectile.Poll<EnemyBullet>().With(p => {
						p.Source = this;
						p.GlobalPosition = world.ToLocal(cannon.GlobalPosition);
						p.Rotation = Game.Player.GlobalPosition.AngleToPoint(cannon.GlobalPosition) + Mathf.Deg2Rad(90);
					}));
				}

				return true;
			}

			return false;
		}

		protected override void Move() {
			if (_timeLeft == 0) {
				var world = Game.GetSafeArea().GetNode<Node2D>("World");

				_fac = 0;
				_timeLeft = 1000;
				_prevSpot = _tgtSpot;
				_tgtSpot = world.ToLocal(Game.GetSafeArea().GetGlobalRect().End * new Vector2(Utils.Rng.Randf(), Utils.Rng.Randf()));
			} else {
				if (_fac >= 1) {
					_timeLeft--;
				} else {
					_fac = Mathf.Clamp(_fac + Speed, 0, 1);
					Position = _prevSpot.LinearInterpolate(_tgtSpot, _fac);
				}
			}
		}
	}
}
