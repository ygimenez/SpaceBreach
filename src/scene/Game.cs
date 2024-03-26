using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using SpaceBreach.entity.interfaces;
using SpaceBreach.entity.misc;
using SpaceBreach.entity.model;
using SpaceBreach.util;

namespace SpaceBreach.scene {
	public abstract class Game : Control {
		private readonly Semaphore _spawnPool = new Semaphore();

		private readonly List<Type> _enemies = typeof(Enemy).Assembly
			.GetTypes()
			.Where(t => t.IsSubclassOf(typeof(Enemy)))
			.ToList();

		private Player _player;
		public ulong Tick;
		private uint _score;
		public uint Score {
			get => _score;
			set => _score = (uint) Mathf.Max(0, value);
		}

		public uint Level = 1;

		public override void _Ready() {
			GetNode<Area2D>("GameArea/Area2D").AddCollision();
			GetNode<Area2D>("GameArea/SafeArea/Area2D").AddCollision(false);

			var world = GetSafeArea().GetNode<Node2D>("World");
			world.AddChild(GD.Load<PackedScene>("res://src/entity/player/Fighter.tscn").Instance<Player>().With(p => {
				_player = p;
				_player.GlobalPosition = world.ToLocal(GetNode<Control>("Spawn").GetGlobalRect().GetCenter());
			}));

			_spawnPool.Post();
		}

		public override void _Process(float delta) {
			Input.MouseMode = GetNode<Control>("PauseMenu").Visible ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured;

			GetNode<Label>("Player1Stats").Text = $@"
			HP: {_player.GetHp()}/{_player.BaseHp}
			Special: {(_player.SpCd.Ready() ? "READY!" : $"[{Utils.PrcntBar(_player.SpCd.Charge(), 8)}]")}
			Score: {Score}";

			if (Utils.Rng.Randfn() > 0.995 && _spawnPool.TryWait() == Error.Ok) {
				var world = GetSafeArea().GetNode<Node2D>("World");

				world.AddChild(Utils.Load(_enemies.Random()).Instance<Enemy>().With(p => {
					var spawn = GetNode<Control>("EnemySpawn").GetGlobalRect();
					p.GlobalPosition = world.ToLocal(spawn.Position + spawn.Size * new Vector2(Utils.Rng.Randf(), 0));

					world.AddChild(GD.Load<PackedScene>("res://src/entity/misc/Marker.tscn").Instance<Marker>().With(m => {
						m.Tracked = p;
					}));
				}));
			}
		}

		public override void _PhysicsProcess(float delta) {
			Tick++;
			if (Tick % 1000 == 0) {
				_spawnPool.Post();
			}

			GetSafeArea().GetNode<CPUParticles2D>("Stars").With(s => {
				s.SpeedScale = _player.SpeedMult * 2;
				(s.Texture as AtlasTexture).Margin = new Rect2(Vector2.Zero, new Vector2(0, -20 + 100 * (1 - _player.SpeedMult)));
			});
		}

		public Control GetSafeArea() {
			return GetNode<Control>("GameArea/SafeArea");
		}

		public bool IsGameOver() {
			return !IsInstanceValid(_player);
		}
	}
}
