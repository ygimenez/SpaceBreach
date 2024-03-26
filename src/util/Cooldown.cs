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

		public uint Time;

		public Cooldown(Game game, uint time) {
			_game = game;
			Time = time;
		}

		public bool Ready() {
			if (_game.IsGameOver()) return false;

			return _game.Tick - _lastTick >= Time;
		}

		public void Use() {
			if (Ready()) {
				_lastTick = _game.Tick;
			}
		}

		public ulong Remaining() {
			return (ulong) Mathf.Max(0, Time - (_game.Tick - _lastTick));
		}

		public float Charge() {
			return (float) (Time - Remaining()) / Time;
		}

		public int IntCharge() {
			return (int) ((Time - Remaining()) * 100 / Time);
		}

		public void Reset() {
			_lastTick = 0;
		}
	}
}
