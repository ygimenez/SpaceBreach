using Godot;
using SpaceBreach.scene;

namespace SpaceBreach.entity.model {
	public abstract class Entity : Area2D {
		[Export]
		public uint BaseHp;

		private uint _hp;
		public uint Hp {
			get => _hp;
			set {
				_hp = value;
				if (_hp <= 0) {
					QueueFree();
					OnDestroy();
				}
			}
		}

		protected Entity(uint baseHp) {
			BaseHp = _hp = baseHp;
		}

		protected Game GetGame() {
			return GetNode<Game>("/root/Control");
		}

		protected virtual void OnDestroy() {
		}
	}
}
