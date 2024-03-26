using Godot;
using SpaceBreach.scene;

namespace SpaceBreach.entity.model {
	public abstract class Entity : Area2D {
		[Export]
		public uint BaseHp;

		private uint _hp;
		protected uint Hp {
			get => _hp;
			set {
				_hp = (uint) Mathf.Max(0, value);
				if (_hp == 0) {
					QueueFree();
					OnDestroy();
				}
			}
		}

		protected Entity(uint baseHp) {
			BaseHp = _hp = baseHp;
		}

		public Game GetGame() {
			return GetNode<Game>("/root/Control");
		}

		public uint GetHp() {
			return _hp;
		}

		public void AddHp(Entity source, long value) {
			if (source != this && value < 0) {
				OnDamaged(source);
			}

			_hp = (uint) Mathf.Max(0, _hp + value);
			if (_hp == 0) {
				QueueFree();
				OnDestroy();
			}
		}

		protected virtual void OnDamaged(Entity by) {
		}

		protected virtual void OnDestroy() {
		}
	}
}
