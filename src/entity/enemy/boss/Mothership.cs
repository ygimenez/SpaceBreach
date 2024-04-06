using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using SpaceBreach.entity.interfaces;
using SpaceBreach.entity.model;
using SpaceBreach.entity.projectile;
using SpaceBreach.entity.projectile.splash;
using SpaceBreach.manager;
using SpaceBreach.scene;
using SpaceBreach.util;

namespace SpaceBreach.entity.enemy.boss {
	public abstract class Mothership : Enemy, IBoss {
		private readonly List<Func<Task>> _skills = new List<Func<Task>>();

		public int DefenderAngle;
		public bool Ready { get; private set; }
		public bool Enraged => Hp <= BaseHp / 2;
		private bool _right = Utils.Rng.Randf() > 0.5f;
		private bool _spawned;

		private Func<Task> NextSkill {
			get {
				if (_skills.Count == 0) {
					_skills.Add(LaserCross);
					_skills.Add(MeteorMaze);
					_skills.Add(MortarBarrage);
					_skills.Add(BulletHell);
					_skills.Sort((a, b) => (int) Utils.Rng.Randi());
				}

				try {
					return _skills[0];
				} finally {
					_skills.RemoveAt(0);
				}
			}
		}

		protected Mothership() : base(hp: 3000, speed: 0.15f) {
		}

		public override void _Ready() {
			base._Ready();
			Cooldown.Paused = true;
		}

		protected override async void Move() {
			var parent = this.FindParent<Control>();
			DefenderAngle++;

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
			UseSkill();
			return true;
		}

		private async void UseSkill() {
			Cooldown.Paused = true;
			await NextSkill.Invoke();
			Cooldown.Paused = false;
		}

		protected override async Task OnDestroy() {
			Audio.StopMusic();
			var size = Size;
			for (var i = 0; i < 10; i++) {
				AddChild(Projectile.Poll<PlayerExplosion>(false).With(l => {
					l.Source = this;
					l.Damage = 0;
					l.Position = -size / 2 + size * Utils.Rng.Randf();
					l.Scale *= 1 + Utils.Rng.Randf();
				}));

				Audio.Cue("res://assets/sounds/mini_explosion.wav");
				await this.Delay(500);
			}
			Game.Nuke();

			await base.OnDestroy();
		}

		private async Task LaserCross() {
			var player = Game.Player;

			var safe = Game.GetSafeArea();
			var world = safe.GetNode<Node2D>("World");

			if (Enraged) {
				for (var i = 0; i < 20; i++) {
					if (Dying || Game.IsGameOver()) return;

					var offset = new Vector2(
						safe.RectSize.x * Utils.Rng.RandiRange(0, 1),
						safe.RectSize.y * Utils.Rng.RandiRange(0, 1)
					);

					world.AddChild(Projectile.Poll<EnemyLaser>(false).With(l => {
						l.Source = this;
						l.Position = offset;
						l.Rotation = player.GlobalPosition.AngleToPoint(world.ToGlobal(offset)) + Mathf.Deg2Rad(90);
					}));

					await this.Delay(500);
				}
			} else {
				for (var j = 0; j < 3; j++) {
					if (Dying || Game.IsGameOver()) return;

					for (var i = 0; i < 2; i++) {
						if (Dying || Game.IsGameOver()) return;

						var offset = new Vector2(safe.RectSize.x * i, 0);
						world.AddChild(Projectile.Poll<EnemyLaser>(false).With(l => {
							l.Source = this;
							l.Position = offset;
							l.Rotation = player.GlobalPosition.AngleToPoint(world.ToGlobal(offset)) + Mathf.Deg2Rad(90);
						}));
					}

					await this.Delay(1500);
				}
			}
		}

		private async Task MeteorMaze() {
			var sample = Projectile.Poll<EnemyMeteor>();
			var size = sample.Size;
			sample.Release();

			var safe = Game.GetSafeArea();
			var world = safe.GetNode<Node2D>("World");

			if (Enraged) {
				for (var j = 0; j < 4; j++) {
					if (Dying || Game.IsGameOver()) return;

					var holes = 5;
					var adjHole = false;

					var radius = Mathf.Max(safe.RectSize.x, safe.RectSize.y);

					for (var i = 0; i < 36; i++) {
						if (Dying || Game.IsGameOver()) return;

						var hole = holes > 0 && Utils.Rng.Randf() > 0.75;
						if (adjHole || hole || i >= 36 - holes) {
							holes--;
							adjHole = !adjHole;

							continue;
						}

						var offset = new Vector2(
							Utils.FCos(10 * i) * radius,
							Utils.FSin(10 * i) * radius
						);
						world.AddChild(Projectile.Poll<EnemyMeteor>().With(l => {
							l.Source = this;
							l.Position = safe.RectSize / 2 + offset;
							l.Target = safe.RectSize / 2;
						}));
					}

					await this.Delay(5000);
				}
			} else {
				var lastDir = -1;
				var direction = -1;
				var amount = (int) safe.RectSize.x / Mathf.Max(size.x, size.y);

				for (var j = 0; j < 4; j++) {
					if (Dying || Game.IsGameOver()) return;

					while (direction == lastDir) {
						direction = Utils.Rng.RandiRange(0, 3);
					}

					var dir = direction;
					lastDir = direction;

					var holes = 3;
					var adjHole = false;

					for (var i = 0; i < amount; i++) {
						if (Dying || Game.IsGameOver()) return;

						var hole = holes > 0 && Utils.Rng.Randf() > 0.75;
						if (adjHole || hole || i >= amount - holes) {
							holes--;
							adjHole = !adjHole;

							continue;
						}

						var offset = Vector2.Zero;
						switch (direction) {
							case 0:
								offset = new Vector2(size.x * i, safe.RectSize.y + size.x * 3);
								break;
							case 1:
								offset = new Vector2(-size.x * 3, size.y * i);
								break;
							case 2:
								offset = new Vector2(size.x * i, -size.x * 3);
								break;
							case 3:
								offset = new Vector2(safe.RectSize.y + size.x * 3, size.y * i);
								break;
						}

						world.AddChild(Projectile.Poll<EnemyMeteor>().With(l => {
							l.Source = this;
							l.Position = offset;
							l.RotationDegrees = 90 * dir;
						}));
					}

					await this.Delay(3000);
				}
			}
		}

		private async Task MortarBarrage() {
			var player = Game.Player;

			var safe = Game.GetSafeArea();
			var world = safe.GetNode<Node2D>("World");

			if (Enraged) {
				var sample = Projectile.Poll<EnemyMortar>(false);
				var size = sample.Size;
				sample.Release();

				var hitPos = Vector2.Zero;
				world.AddChild(Projectile.Poll<EnemyMortar>(false).With(l => {
					l.Source = this;
					l.Position = hitPos = player.Position;
				}));

				var amount = (int) safe.RectSize.x / Mathf.Max(size.x, size.y);
				for (var i = 0; i < amount; i++) {
					if (Dying || Game.IsGameOver()) return;

					for (var j = 0; j < 4; j++) {
						if (Dying || Game.IsGameOver()) return;

						var offset = Vector2.Zero;
						switch (j) {
							case 0:
								offset = new Vector2(0, size.y);
								break;
							case 1:
								offset = new Vector2(size.x, 0);
								break;
							case 2:
								offset = new Vector2(0, -size.y);
								break;
							case 3:
								offset = new Vector2(-size.x, 0);
								break;
						}

						var distance = i;
						world.AddChild(Projectile.Poll<EnemyMortar>(false).With(l => {
							l.Source = this;
							l.Position = hitPos + offset * distance;
						}));
					}

					await this.Delay(250);
				}
			} else {
				for (var i = 0; i < 10; i++) {
					if (Dying || Game.IsGameOver()) return;

					world.AddChild(Projectile.Poll<EnemyMortar>(false).With(l => {
						l.Source = this;
						l.Position = player.Position + player.Size * new Vector2(Utils.Rng.Randf() - 0.5f, Utils.Rng.Randf() - 0.5f) * 5;
					}));

					await this.Delay(500);
				}
			}
		}

		private async Task BulletHell() {
			var safe = Game.GetSafeArea();
			var world = safe.GetNode<Node2D>("World");

			if (Enraged) {
				var invert = false;
				for (var j = 0; j < 25; j++) {
					if (Dying || Game.IsGameOver()) return;

					var rotSpeed = invert ? -0.5f : 0.5f;

					for (var i = 0; i < 8; i++) {
						if (Dying || Game.IsGameOver()) return;

						var ang = 45 * i;
						world.AddChild(Projectile.Poll<EnemySpirallingOrb>().With(l => {
							l.Source = this;
							l.Position = Position;
							l.RotationDegrees = ang;
							l.RotationSpeed = rotSpeed;
						}));
					}

					invert = !invert;
					await this.Delay(500);
				}
			} else {
				for (var j = 0; j < 50; j++) {
					if (Dying || Game.IsGameOver()) return;

					for (var i = 0; i < 3; i++) {
						if (Dying || Game.IsGameOver()) return;

						var ang = 60 + 30 * i;
						world.AddChild(Projectile.Poll<EnemySpirallingOrb>().With(l => {
							l.Source = this;
							l.Position = Position;
							l.RotationDegrees = ang;
							l.RotationSpeed = 0;
						}));
					}

					await this.Delay(250);
				}
			}
		}
	}
}
