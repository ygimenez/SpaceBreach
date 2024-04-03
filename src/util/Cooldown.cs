using Godot;
using SpaceBreach.entity.model;
using SpaceBreach.scene;

namespace SpaceBreach.util {
	public class Cooldown {
		private readonly Game _game;
		private readonly Entity _source;

		private ulong _lastTick;
		private bool _first = true;
		private ulong _pauseTick;
		public bool Paused {
			get => _pauseTick > 0;
			set {
				if (value == Paused) return;

				if (value) {
					_pauseTick = _game.Tick;
				} else {
					_lastTick += _game.Tick - _pauseTick;
					_pauseTick = 0;
				}
			}
		}

		public uint Time;

		public Cooldown(Game game, Entity source, uint time) {
			_game = game;
			_source = source;
			Time = time;
		}

		public bool Ready() {
			if (_source.Dying || _game.IsGameOver() || Paused) return false;

			return _first || _game.Tick - _lastTick >= Time / _source.ActionSpeed;
		}

		public void Use() {
			_lastTick = _game.Tick;
			_first = false;
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
			_first = true;
		}
	}
}
