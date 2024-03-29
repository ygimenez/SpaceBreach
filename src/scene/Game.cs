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
		public uint Highscore;
		public uint TextLeft;
		public uint Streak;
		public Enemy Boss;

		public override void _Ready() {
			CallDeferred(nameof(_Initialize));
			if (Global.Online && Global.Leaderboard.Count > 0) {
				Highscore = Global.Leaderboard[0].Item2;
			}
		}

		public void _Initialize() {
			GetNode<Area2D>("GameArea/Area2D").AddCollision();
			GetNode<Area2D>("GameArea/MaxSizeContainer/SafeArea/Area2D").AddCollision(false);

			if (Global.Mobile) {
				GetNode<Button>("Pause").Connect("pressed", this, nameof(_PausePressed));
			} else {
				GetNode<Control>("Mobile").QueueFree();
				GetNode<Control>("Pause").QueueFree();
			}

			var world = GetSafeArea().GetNode<Node2D>("World");
			world.AddChild(GD.Load<PackedScene>("res://src/entity/player/Fighter.tscn").Instance<Player>().With(p => {
				_player = p;
				_player.GlobalPosition = world.ToLocal(GetNode<Control>("GameArea/Spawn").GetGlobalRect().GetCenter());
			}));

			world.AddChild(GD.Load<PackedScene>("res://src/entity/misc/FallingText.tscn").Instance<FallingText>().With(t => {
				t.Position = new Vector2(10, 0);
				t.GetNode<AutoFont>("Label").TestString = "Press SPACE to shoot";

				if (Global.Mobile) {
					t.Text = "<- Slide to move";
				} else {
					t.Text = $"Press {((InputEvent) InputMap.GetActionList("shoot")[0]).AsText().ToUpper()} to shoot";
				}
			}));

			world.AddChild(GD.Load<PackedScene>("res://src/entity/misc/FallingText.tscn").Instance<FallingText>().With(t => {
				t.Position = world.GetParent<Control>().RectSize * new Vector2(1, -0.5f) + new Vector2(-10, 0);
				t.Origin = Vector2.Right;
				t.GetNode<AutoFont>("Label").TestString = "Press SPACE to shoot";

				if (Global.Mobile) {
					t.Text = @"
					Single Tap to shoot    ->
					Double tap for special ->";
				} else {
					t.Text = $"Press {((InputEvent) InputMap.GetActionList("special")[0]).AsText().ToUpper()} for special";
				}
			}));

			world.AddChild(GD.Load<PackedScene>("res://src/entity/misc/FallingText.tscn").Instance<FallingText>().With(t => {
				t.Position = world.GetParent<Control>().RectSize * new Vector2(0.5f, -1.1f);
				t.Origin = new Vector2(0.5f, 0);

				t.Text = "Good luck!";
			}));

			Audio.PlayMusic("res://assets/sounds/music/main.tres");
		}

		public override void _Process(float delta) {
			var stats = GetNode<Label>("GameArea/PlayerStats");
			stats.Visible = !IsGameOver();
			stats.Text = $@"
			HP: {_player.GetHp()}/{_player.BaseHp}
			Special: {(_player.SpCd.Ready() ? "READY!" : $"[{Utils.PrcntBar(_player.SpCd.Charge(), 8)}]")}
			Score (x{decimal.Round((decimal) (1 + Streak / 10f), 1)}): {Score}
			{(Highscore > 0 ? $"Highscore: {Highscore}" : "")}
			".Trim();

			if (!IsGameOver() && Boss == null && Utils.Rng.Randfn() > 0.995 && SpawnPool > 0) {
				var world = GetSafeArea().GetNode<Node2D>("World");

				world.AddChild(Utils.Load(_enemies.Random()).Instance<Enemy>().With(e => {
					var spawn = GetNode<Control>("GameArea/MaxSizeContainer2/EnemySpawn").GetGlobalRect();
					e.GlobalPosition = world.ToLocal(spawn.Position + spawn.Size * new Vector2(Utils.Rng.Randf(), 0.25f));

					world.AddChild(GD.Load<PackedScene>("res://src/entity/misc/Marker.tscn").Instance<Marker>().With(m => {
						m.Tracked = e;
					}));

					if (e is IBoss) {
						Boss = e;
						GetNode<AnimationPlayer>("Warning/AnimationPlayer").Play("Flash");
						Audio.Cue("res://assets/sounds/warning.wav");
					}

					SpawnPool--;
				}));
			}
		}

		public override void _PhysicsProcess(float delta) {
			if (TextLeft == 0) {
				if (Tick++ % 2000 == 0) {
					SpawnPool++;
				}
			}

			GetSafeArea().GetNode<CPUParticles2D>("Stars").With(s => {
				s.SpeedScale = _player.Speed;
				(s.Texture as AtlasTexture).Margin = new Rect2(Vector2.Zero, new Vector2(0, -20 + 100 * _player.Speed / 10));
			});
		}

		public override void _ExitTree() {
			Audio.StopMusic();
		}

		public void PlayerDeath(Player p) {
			if (IsGameOver()) {
				Audio.StopMusic();
				Audio.Cue("res://assets/sounds/gameover_ramp.wav");
				GetTree().CreateTimer(2).Connect("timeout", this, nameof(_ShowGameOver));
			}
		}

		public void _ShowGameOver() {
			GetTree().Paused = true;
			GetNode<Control>("PauseMenu").Visible = true;
			if (Global.Online && Score > Highscore) {
				GetNode<Control>("PauseMenu/Leaderboard").Visible = true;
				GetNode<Control>("PauseMenu/Back").Visible = false;
			}

			Audio.Cue("res://assets/sounds/gameover.wav");
		}

		public void _PausePressed() {
			var menu = GetNode<PauseMenu>("PauseMenu");
			menu.Visible = !menu.Visible;
		}

		public Control GetSafeArea() {
			return GetNode<Control>("GameArea/MaxSizeContainer/SafeArea");
		}

		public bool IsGameOver() {
			return !IsInstanceValid(_player) || _player.GetHp() == 0;
		}
	}
}
