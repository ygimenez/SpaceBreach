using Godot;
using SpaceBreach.scene;

namespace SpaceBreach.util {
	public class CostCooldown {
		private readonly Game _game;

		public uint Credits, Cost;

		public CostCooldown(Game game, uint cost) {
			_game = game;
			Cost = Credits = cost;
		}

		public bool Ready() {
			if (_game.IsGameOver()) return false;

			return Credits >= Cost;
		}

		public void Use() {
			if (Ready()) {
				Credits = 0;
			}
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
