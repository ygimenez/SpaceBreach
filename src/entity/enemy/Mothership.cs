using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using SpaceBreach.entity.interfaces;
using SpaceBreach.entity.model;
using SpaceBreach.entity.projectile;
using SpaceBreach.util;

namespace SpaceBreach.entity.enemy {
	public abstract class Mothership : Enemy, IBoss {
		private readonly List<Func<Task>> _skills = new List<Func<Task>>();

		public int DefAngle;
		public bool Ready { get; private set; }
		public bool Enraged => Hp <= BaseHp / 2;
		private bool _right = Utils.Rng.Randf() > 0.5f;
		private bool _spawned;

		private Func<Task> NextSkill {
			get {
				if (_skills.Count == 0) {
					// _skills.Add(LaserCross);
					_skills.Add(MeteorMaze);
					_skills.Sort((a, b) => (int) Utils.Rng.Randi());
				}

				try {
					return _skills[0];
				} finally {
					_skills.RemoveAt(0);
				}
			}
		}

		protected Mothership() : base(hp: 3000, 0.1f, speed: 0.15f) {
		}

		public override void _Ready() {
			base._Ready();
			Cooldown.Paused = true;
		}

		protected override async void Move() {
			var parent = this.FindParent<Control>();
			DefAngle++;

			if (Position.y < parent.RectSize.y / 20) {
				Translate(new Vector2(0, Speed));
			} else if (!_spawned) {
				_spawned = true;

				var def = GD.Load<PackedScene>("res://src/entity/enemy/Defender.tscn");
				for (var i = 0; i < 10; i++) {
					AddChild(def.Instance<Defender>());
					await this.Delay(500 / ActionSpeed);
				}

				Ready = true;
				Cooldown.Paused = false;
			}

			if (Ready) {
				if (Position.x < parent.RectSize.x * 0.3f && !_right || Position.x > parent.RectSize.x * 0.6f && _right) {
					_right = !_right;
				}

				Translate(new Vector2(Speed / 2 * (_right ? 1 : -1), 0));
			}
		}

		protected override bool Shoot() {
			Cooldown.Paused = true;
			NextSkill.Invoke()
				.ContinueWith(_ => Cooldown.Paused = false);

			return true;
		}

		protected override async Task OnDestroy() {
			foreach (Node child in GetChildren()) {
				if (child is Entity || child is Projectile) {
					child.QueueFree();
				}
			}

			await this.Delay(5000 / ActionSpeed);
			await base.OnDestroy();
		}

		private async Task LaserCross() {
			var player = GetGame().Player;

			var laser = GD.Load<PackedScene>("res://src/entity/projectile/splash/EnemyLaser.tscn");
			var safe = GetGame().GetSafeArea();
			var world = safe.GetNode<Node2D>("World");

			if (Enraged) {
				for (var i = 0; i < 20; i++) {
					var offset = new Vector2(
						safe.RectSize.x * Utils.Rng.RandiRange(0, 1),
						safe.RectSize.y * Utils.Rng.RandiRange(0, 1)
					);
					if (GetGame().IsGameOver()) return;

					world.AddChild(laser.Instance<Projectile>().With(l => {
						l.Source = this;
						l.Position = offset;
						l.Rotation = player.GlobalPosition.AngleToPoint(world.ToGlobal(offset)) + Mathf.Deg2Rad(90);
					}));

					await this.Delay(500);
				}
			} else {
				for (var i = 0; i < 2; i++) {
					var offset = new Vector2(safe.RectSize.x * i, 0);
					world.AddChild(laser.Instance<Projectile>().With(l => {
						l.Source = this;
						l.Position = offset;
						l.Rotation = player.GlobalPosition.AngleToPoint(world.ToGlobal(offset)) + Mathf.Deg2Rad(90);
					}));
				}
			}
		}

		private async Task MeteorMaze() {
			var meteor = GD.Load<PackedScene>("res://src/entity/projectile/EnemyMeteor.tscn");
			var sample = meteor.Instance<EnemyMeteor>();
			var size = sample.Size;
			sample.QueueFree();

			var safe = GetGame().GetSafeArea();
			var world = safe.GetNode<Node2D>("World");

			if (Enraged) {

			} else {
				var amount = (int) safe.RectSize.x / size.x;
				var holes = 0;

				for (var i = 0; i < amount; i++) {
					var hole = holes < 3 && Utils.Rng.Randfn() > 0.5;
					if (hole || i >= amount - (3 - holes)) {
						holes++;
						continue;
					}

					var offset = new Vector2(size.x * i, -size.x * 3);
					world.AddChild(meteor.Instance<Projectile>().With(l => {
						l.Source = this;
						l.Position = offset;
						l.RotationDegrees = 180;
					}));
				}
			}
		}
	}
}
