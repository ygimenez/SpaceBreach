using Godot;
using SpaceBreach.scene;
using SpaceBreach.util;

namespace SpaceBreach.entity.model {
	public abstract class Entity : Area2D {
		[Export]
		public uint BaseHp;

		[Export]
		public float Speed {
			get => RawSpeed * ActionSpeed;
			set => RawSpeed = value;
		}

		[Export]
		public float ActionSpeed {
			get => RawActionSpeed * Global.ACTION_SPEED;
			set => RawActionSpeed = value;
		}

		[Signal]
		public delegate void Death(Entity entity);

		protected uint Hp;
		public float RawSpeed;
		public float RawActionSpeed = 1;
		public new bool Visible;

		protected Entity(uint baseHp, float speed = 1) {
			BaseHp = Hp = baseHp;
			Speed = speed;
		}

		protected Game GetGame() {
			return GetNode<Game>("/root/Control");
		}

		public uint GetHp() {
			return Hp;
		}

		public void AddHp(Entity source, long value) {
			if (this is Enemy && !Visible) return;
			if (source != this && value < 0) {
				OnDamaged(source);
			}

			Hp = (uint) (Hp + value).Clamp(0, BaseHp);
			if (Hp == 0) {
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
