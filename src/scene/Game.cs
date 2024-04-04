using System;
using System.Linq;
using Godot;
using SpaceBreach.entity.interfaces;
using SpaceBreach.entity.misc;
using SpaceBreach.entity.model;
using SpaceBreach.manager;
using SpaceBreach.util;

namespace SpaceBreach.scene {
	public abstract class Game : Control {
		private static readonly WeightedList<Type> _enemies = new WeightedList<Type>();

		static Game() {
			var enemies = typeof(Enemy).Assembly
				.GetTypes()
				.Where(t => t.IsSubclassOf(typeof(Enemy)) && !typeof(ICannotSpawn).IsAssignableFrom(t))
				.Select(t => Utils.Load(t).Instance<Enemy>())
				.ToList();

			var max = enemies
				.Where(e => !(e is IBoss))
				.Max(e => e.GetCost());

			foreach (var enemy in enemies) {
				if (enemy is IBoss) {
					_enemies.Add(enemy.GetType(), int.MaxValue);
				} else {
					_enemies.Add(enemy.GetType(), (int) (max - enemy.GetCost()));
				}
			}
		}

		private bool _online;
		private uint _score;
		public ulong Tick;
		public ulong SpawnTick;
		public uint Score {
			get => _score;
			set => _score = (uint) Mathf.Max(0, value);
		}

		public Player Player;
		public uint SpawnPool;
		public uint Highscore;
		public uint TextLeft;
		public uint Streak;
		public Enemy Boss;

		private uint _level = 1;
		public uint Level {
			get => _level;
			set {
				if (value > _level) {
					var back = GetNode<ColorRect>("Background");
					var rng = new RandomNumberGenerator { Seed = (ulong) value.GetHashCode() };

					var tween = CreateTween();
					tween.TweenProperty(back, "color", new Color(
						rng.Randf() / 3, rng.Randf() / 3, rng.Randf() / 3
					), 2);
				}

				_level = value;
			}
		}

		public override void _Ready() {
			_online = Global.Online;

			CallDeferred(nameof(_Initialize));
			if (_online && Global.Leaderboard.Count > 0) {
				Highscore = Global.Leaderboard[0].Item2;
			}
		}

		public void _Initialize() {
			GetNode<Area2D>("GameArea/Area2D").AddCollision();
			GetNode<Area2D>("GameArea/MaxSizeContainer/SafeArea/Area2D").AddCollision();
			GetNode<Area2D>("GameArea/MaxSizeContainer/SafeArea/Area2D").AddCollision(false);

			if (Global.Mobile) {
				GetNode<Button>("Pause").Connect("pressed", this, nameof(_PausePressed));
			} else {
				GetNode<Control>("Mobile").QueueFree();
				GetNode<Control>("Pause").QueueFree();
			}

			var world = GetSafeArea().GetNode<Node2D>("World");
			world.AddChild(GD.Load<PackedScene>("res://src/entity/player/Fighter.tscn").Instance<Player>().With(p => {
				Player = p;
				Player.GlobalPosition = world.ToLocal(GetNode<Control>("GameArea/Spawn").GetGlobalRect().GetCenter());
			}));

			world.AddChild(GD.Load<PackedScene>("res://src/entity/misc/FallingText.tscn").Instance<FallingText>().With(t => {
				t.Position = new Vector2(10, 0);
				t.GetNode<AutoFont>("Label").TestString = "Press SPACE to shoot";

				if (Global.Mobile) {
					t.Text = "Slide to move/shoot";
				} else {
					t.Text = $"Press {((InputEvent) InputMap.GetActionList("shoot")[0]).AsText().ToUpper()} to shoot";
				}
			}));

			world.AddChild(GD.Load<PackedScene>("res://src/entity/misc/FallingText.tscn").Instance<FallingText>().With(t => {
				t.Position = world.GetParent<Control>().RectSize * new Vector2(1, -0.5f) + new Vector2(-10, 0);
				t.Origin = Vector2.Right;
				t.GetNode<AutoFont>("Label").TestString = "Press SPACE to shoot";

				if (Global.Mobile) {
					t.Text = "Double Tap for special";
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
			HP: {Player.GetHp()}/{Player.BaseHp}
			Special: {(Player.SpCd.Ready() ? "READY!" : $"[{Utils.PrcntBar(Player.SpCd.Charge(), 8)}]")}
			Score (x{decimal.Round((decimal) (1 + Streak / 10f), 1)}): {Score}
			Level: {Level}
			{(Highscore > 0 ? $"Highscore: {Highscore}" : "")}
			".Trim();

			if (!IsGameOver() && Boss == null && Utils.Rng.Randf() > 0.95 && SpawnPool > 0) {
				var world = GetSafeArea().GetNode<Node2D>("World");

				var chosen = _enemies.Next();
				if (typeof(ICannotSpawn).IsAssignableFrom(chosen)) return;
				if (typeof(IBoss).IsAssignableFrom(chosen) != (SpawnTick / 10_000 == Level)) return;

				world.AddChild(Utils.Load(chosen).Instance<Enemy>().With(e => {
					var spawn = GetNode<Control>("GameArea/MaxSizeContainer2/EnemySpawn").GetGlobalRect();
					e.GlobalPosition = world.ToLocal(spawn.Position + spawn.Size * new Vector2(Utils.Rng.Randf(), 0.25f));

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
				Tick++;

				if (SpawnTick % 2000 == 0) {
					SpawnPool++;
				}

				SpawnTick = Math.Min(SpawnTick + 1, Level * 10_000);
			}

			GetSafeArea().GetNode<CPUParticles2D>("Stars").With(s => {
				s.SpeedScale = Player.Speed;
				(s.Texture as AtlasTexture).Margin = new Rect2(Vector2.Zero, new Vector2(0, -20 + 100 * Player.Speed / 10));
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
			if (_online && Score > Highscore) {
				GetNode<Control>("PauseMenu/Leaderboard").Visible = true;
				GetNode<Control>("PauseMenu/Back").Visible = false;
			}

			Audio.Cue("res://assets/sounds/gameover.wav");
		}

		public void _PausePressed() {
			if (!IsGameOver()) {
				var menu = GetNode<PauseMenu>("PauseMenu");
				menu.Visible = !menu.Visible;
			}
		}

		public Control GetSafeArea() {
			return GetNode<Control>("GameArea/MaxSizeContainer/SafeArea");
		}

		public void Nuke() {
			var world = GetSafeArea().GetNode<Node2D>("World");
			foreach (Node child in world.GetChildren()) {
				switch (child) {
					case IBoss _:
						continue;
					case Enemy e:
						e.Kill();
						break;
					case Projectile p:
						p.Release();
						break;
				}
			}

			var rect = GetNode<ColorRect>("ColorRect");
			rect.Color = Colors.White;
			CreateTween().TweenProperty(rect, "color", new Color(Colors.White, 0), 2);
		}

		public bool IsGameOver() {
			return !IsInstanceValid(Player) || Player.GetHp() == 0;
		}
	}
}
