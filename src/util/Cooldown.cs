using Godot;
using SpaceBreach.scene;

namespace SpaceBreach.util {
	public class Cooldown {
		private readonly Game _game;
		private ulong _lastTick;

		private ulong _pauseTick;
		private bool Paused {
			get => _pauseTick > 0;
			set {
				if (value == Paused) return;

				if (value) {
					_lastTick += _game.Tick - _pauseTick;
					_pauseTick = 0;
				} else {
					_pauseTick = _game.Tick;
				}
			}
		}

		public Cooldown(Game game) {
			_game = game;
		}

		public bool Use(ulong cooldown) {
			if (_game.Tick - _lastTick >= cooldown) {
				_lastTick = _game.Tick;
				return true;
			}

			return false;
		}

		public ulong Remaining(ulong cooldown) {
			return (ulong) Mathf.Max(0, cooldown - (_game.Tick - _lastTick));
		}
	}
}
