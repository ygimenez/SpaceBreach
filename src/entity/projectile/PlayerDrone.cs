using Godot;
using SpaceBreach.entity.interfaces;
using SpaceBreach.entity.model;
using SpaceBreach.entity.particle;
using SpaceBreach.entity.player;
using SpaceBreach.manager;
using SpaceBreach.util;

namespace SpaceBreach.entity.projectile {
	public abstract class PlayerDrone : Projectile {
		private bool _defending;
		private int _index, _drones, _bounces = 5;
		public uint Cooldown;

		protected PlayerDrone() : base(0) {
		}

		public override void _Ready() {
			var cont = GD.Load<PackedScene>("res://src/entity/particle/Trail.tscn");
			var world = Game.GetSafeArea().GetNode<Node2D>("World");

			world.AddChild(cont.Instance<Trail>().With(t => {
				t.Source = GetNode<Position2D>("Trail");
			}));

			Connect("area_shape_entered", this, nameof(_AreaShapeEntered));

			var parent = GetParent();
			if (!(parent is Carrier carrier)) return;

			Cooldown = (uint) (200 / carrier.AttackRate);
		}

		public override void _Process(float delta) {
			var parent = GetParent();
			GetNode<Position2D>("Trail").Visible = !(parent is Carrier);
			if (!(parent is Carrier carrier)) return;

			Visible = Cooldown == 0;
			_drones = 0;

			var i = 0;
			foreach (var child in carrier.GetChildren()) {
				if (child is PlayerDrone) {
					_drones++;

					if (child == this) {
						_index = i;
					}

					i++;
				}
			}

			if (_defending != carrier.Defending) {
				_defending = carrier.Defending;

				var anim = GetNode<AnimationPlayer>("AnimationPlayer");
				if (_defending) {
					anim.Play("Deploy");
				} else {
					anim.PlayBackwards("Deploy");
				}
			}
		}

		public override void _PhysicsProcess(float delta) {
			base._PhysicsProcess(delta);

			var parent = GetParent();
			if (!(parent is Carrier carrier)) return;

			var size = carrier.Size;
			var radius = Mathf.Max(size.y, size.y) * 1.5f;

			RotationDegrees = carrier.DroneAngle + 360f / _drones * _index + 90;
			Position = new Vector2(
				Utils.FCos((int) (carrier.DroneAngle + 360f / _drones * _index)) * radius,
				Utils.FSin((int) (carrier.DroneAngle + 360f / _drones * _index)) * radius
			);
		}

		protected override void OnEntityHit(Entity entity) {
			var parent = GetParent();
			if (!(parent is Carrier carrier)) {
				if (entity != null && Damage > 0) {
					entity.AddHp(Source, (long) (-Damage * (1 + 0.1f * (5 - _bounces))));
				}

				if (_bounces == 0) {
					Release();
				}

				return;
			}

			if (_defending || Cooldown > 0) return;

			if (entity != null && Damage > 0) {
				entity.AddHp(Source, -Damage);
			}

			Cooldown = (uint) (200 / carrier.AttackRate);
		}

		protected override void OnProjectileHit(Projectile proj) {
			var parent = GetParent();
			if (!(parent is Carrier carrier)) {
				base.OnProjectileHit(proj);
				return;
			}

			if (!_defending || Cooldown > 0 || proj is ISplash) return;

			if (proj.Source is Enemy) {
				proj.Release();
			}

			Cooldown = (uint) (200 / carrier.AttackRate);
		}

		public void _AreaShapeEntered(RID _, Area2D entity, int __, int ___) {
			if (GetParent() is Carrier || _bounces == 0) return;

			var ray = GetNode<RayCast2D>("RayCast2D");
			ray.Enabled = true;
			ray.ForceRaycastUpdate();

			if (ray.IsColliding()) {
				MovementVector = MovementVector.Bounce(ray.GetCollisionNormal());
				_bounces--;
				Audio.Cue("res://assets/sounds/ricochet.wav");
			}

			ray.Enabled = false;
		}
	}
}
