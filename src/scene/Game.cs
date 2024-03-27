using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using SpaceBreach.entity.interfaces;
using SpaceBreach.entity.misc;
using SpaceBreach.entity.model;
using SpaceBreach.manager;
using SpaceBreach.util;

namespace SpaceBreach.scene {
	public abstract class Game : Control {
		private readonly List<Type> _enemies = typeof(Enemy).Assembly
			.GetTypes()
			.Where(t => t.IsSubclassOf(typeof(Enemy)))
			.ToList();

		private Player _player;
		private uint _score;

		public ulong Tick;
		public uint Score {
			get => _score;
			set => _score = (uint) Mathf.Max(0, value);
		}

		public uint SpawnPool;
		public uint Level = 1;
		public Enemy Boss;

		public override void _Ready() {
			GetNode<Area2D>("GameArea/Area2D").AddCollision();
			GetNode<Area2D>("GameArea/SafeArea/Area2D").AddCollision(false);
			Audio.PlayMusic("res://assets/sounds/music/main.tres");

			var world = GetSafeArea().GetNode<Node2D>("World");
			world.AddChild(GD.Load<PackedScene>("res://src/entity/player/Fighter.tscn").Instance<Player>().With(p => {
				_player = p;
				_player.GlobalPosition = world.ToLocal(GetNode<Control>("Spawn").GetGlobalRect().GetCenter());
			}));


		}

		public override void _Process(float delta) {
			GetNode<Label>("Player1Stats").Text = $@"
			HP: {_player.GetHp()}/{_player.BaseHp}
			Special: {(_player.SpCd.Ready() ? "READY!" : $"[{Utils.PrcntBar(_player.SpCd.Charge(), 8)}]")}
			Score: {Score}";

			if (!IsGameOver() && Boss == null && Utils.Rng.Randfn() > 0.995 && SpawnPool > 0) {
				var world = GetSafeArea().GetNode<Node2D>("World");

				world.AddChild(Utils.Load(_enemies.Random()).Instance<Enemy>().With(p => {
					var spawn = GetNode<Control>("EnemySpawn").GetGlobalRect();
					p.GlobalPosition = world.ToLocal(spawn.GetCenter() + spawn.Size * new Vector2(Utils.Rng.Randf() - 0.5f, 0));

					world.AddChild(GD.Load<PackedScene>("res://src/entity/misc/Marker.tscn").Instance<Marker>().With(m => {
						m.Tracked = p;
					}));

					if (p is IBoss) {
						Boss = p;
						GetNode<AnimationPlayer>("Warning/AnimationPlayer").Play("Flash");
						Audio.Cue("res://assets/sounds/warning.wav");
					}

					SpawnPool--;
				}));
			}
		}

		public override void _PhysicsProcess(float delta) {
			Tick++;
			if (Tick % 1000 == 0) {
				SpawnPool++;
			}

			GetSafeArea().GetNode<CPUParticles2D>("Stars").With(s => {
				s.SpeedScale = _player.Speed;
				(s.Texture as AtlasTexture).Margin = new Rect2(Vector2.Zero, new Vector2(0, -20 + 100 * (1 - _player.Speed / 2)));
			});
		}

		public void PlayerDeath(Player p) {
			GD.Print("sus");
			if (IsGameOver()) {
				Audio.StopMusic();
				Audio.Cue("res://assets/sounds/gameover_ramp.wav");
				GetTree().CreateTimer(2).Connect("timeout", this, nameof(_ShowGameOver));
			}
		}

		public void _ShowGameOver() {
			GetNode<Control>("PauseMenu").Visible = true;
			Audio.Cue("res://assets/sounds/gameover.wav");
		}

		public Control GetSafeArea() {
			return GetNode<Control>("GameArea/SafeArea");
		}

		public bool IsGameOver() {
			return !IsInstanceValid(_player) || _player.GetHp() == 0;
		}
	}
}
