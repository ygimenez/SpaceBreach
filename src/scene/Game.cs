using Godot;
using SpaceBreach.entity.model;
using SpaceBreach.util;

namespace SpaceBreach.scene {
	public class Game : Control {
		private Player _player;
		public ulong Tick;

		public override void _Ready() {
			GetNode<PanelContainer>("SafeArea").AddChild(
				GD.Load<PackedScene>("res://src/entity/player/Fighter.tscn").Instance().With(p => {
					((Area2D) p).GlobalPosition = GetNode<Control>("Spawn").RectGlobalPosition;
					_player = (Player) p.Get("Self");
					p.Set("Game", this);
				})
			);
		}

		public override void _PhysicsProcess(float delta) {
			Tick++;

			GetNode("SafeArea").GetNode<CPUParticles2D>("Stars").With(s => {
				s.SpeedScale = _player.Speed * 2;
				((AtlasTexture) s.Texture).Margin = new Rect2(Vector2.Zero, new Vector2(0, -20 + 100 * (1 - _player.Speed)));
			});
		}
	}
}
