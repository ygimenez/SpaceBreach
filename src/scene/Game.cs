using Godot;
using SpaceBreach.entity.enemy;
using SpaceBreach.entity.model;
using SpaceBreach.util;

namespace SpaceBreach.scene {
	public abstract class Game : Control {
		private Player _player;
		public ulong Tick;

		public override void _Ready() {
			AddChild(
				GD.Load<PackedScene>("res://src/entity/player/Fighter.tscn").Instance().With(p => {
					_player = (Player) p;
					_player.GlobalPosition = GetNode<Control>("Spawn").GetRect().GetCenter();
				})
			);

			AddChild(
				GD.Load<PackedScene>("res://src/entity/enemy/Invader.tscn").Instance<Invader>().With(p => {
					p.GlobalPosition = GetSafeArea().GetRect().GetCenter();
				})
			);
		}

		public override void _Process(float delta) {
			GetNode<Label>("Player1Stats").Text = $@"
			HP: {_player.GetHp()}/{_player.BaseHp}
			Special: {(_player.SpCd.Ready() ? "READY!" : $"[{Utils.PrcntBar(_player.SpCd.Charge(), 8)}]")}";
		}

		public override void _PhysicsProcess(float delta) {
			Tick++;

			GetSafeArea().GetNode<CPUParticles2D>("Stars").With(s => {
				s.SpeedScale = _player.SpeedMult * 2;
				((AtlasTexture) s.Texture).Margin = new Rect2(Vector2.Zero, new Vector2(0, -20 + 100 * (1 - _player.SpeedMult)));
			});
		}

		public uint GetScore() {
			return 1;
		}

		public uint GetLevel() {
			return 1;
		}

		public Control GetSafeArea() {
			return GetNode<Control>("SafeArea");
		}

		public bool IsGameOver() {
			return !IsInstanceValid(_player);
		}
	}
}
