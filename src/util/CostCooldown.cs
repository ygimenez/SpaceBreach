using Godot;
using SpaceBreach.entity.model;
using SpaceBreach.scene;

namespace SpaceBreach.util {
	public class CostCooldown {
		private readonly Game _game;
		private readonly Entity _source;

		public uint Credits, Cost;

		public CostCooldown(Game game, Entity source, uint cost) {
			_game = game;
			_source = source;
			Cost = Credits = cost;
		}

		public bool Ready() {
			if (_source.Dying || _game.IsGameOver()) return false;
			if (_source is Player p && p.SpecialRate == 0) return true;

			return Credits >= Cost / _source.ActionSpeed;
		}

		public void Use() {
			Credits = 0;
		}

		public ulong Remaining() {
			return (ulong) Mathf.Max(0, Cost - Credits);
		}

		public float Charge() {
			return (float) (Cost - Remaining()) / Cost;
		}

		public int IntCharge() {
			return (int) ((Cost - Remaining()) * 100 / Cost);
		}

		public void Reset() {
			Credits = Cost;
		}
	}
}
