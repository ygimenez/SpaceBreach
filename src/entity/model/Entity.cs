﻿using Godot;
using SpaceBreach.scene;
using SpaceBreach.util;

namespace SpaceBreach.entity.model {
	public abstract class Entity : Area2D {
		[Export]
		public uint BaseHp;

		[Export]
		public float Speed {
			get => _speed * ActionSpeed;
			set => _speed = value;
		}

		[Export]
		public float ActionSpeed {
			get => _actionSpeed * Global.ACTION_SPEED;
			set => _actionSpeed = value;
		}

		[Signal]
		public delegate void Death(Entity entity);

		private float _speed;
		private float _actionSpeed = 1;
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

		public new bool Visible;

		protected Entity(uint baseHp, float speed = 1) {
			BaseHp = _hp = baseHp;
			Speed = speed;
		}

		protected Game GetGame() {
			return GetNode<Game>("/root/Control");
		}

		public uint GetHp() {
			return _hp;
		}

		public void AddHp(Entity source, long value) {
			if (source != this && value < 0) {
				OnDamaged(source);
			}

			_hp = (uint) (_hp + value).Clamp(0, BaseHp);
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
