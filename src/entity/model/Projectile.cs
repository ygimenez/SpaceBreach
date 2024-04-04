using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using SpaceBreach.entity.interfaces;
using SpaceBreach.entity.misc;
using SpaceBreach.scene;
using SpaceBreach.util;

namespace SpaceBreach.entity.model {
	public abstract class Projectile : Area2D {
		private static readonly Dictionary<Type, Queue<Projectile>> Pool = new Dictionary<Type, Queue<Projectile>>();
		protected bool Released = true;

		[Export]
		public float Speed;

		[Export]
		public uint Damage;

		public Vector2 Size => GetNode<Sprite>("Sprite").GetRect().Size;

		protected Game Game => GetNode<Game>("/root/Control");

		public Entity Source;
		public bool Appeared { get; set; }

		protected Projectile(float speed, uint damage) {
			Speed = speed;
			Damage = damage;
		}

		public override void _Process(float delta) {
			if (Speed != 0) {
				GlobalTranslate(Vector2.Up.Rotated(Rotation) * Speed * Global.ACTION_SPEED * Engine.TimeScale);
			}

			foreach (var area in GetOverlappingAreas()) {
				if (area is Entity e) {
					OnHit(e);
				}
			}
		}

		protected virtual void OnHit(Entity entity) {
			if (entity != null && Damage > 0) {
				entity.AddHp(Source, -Damage);
			}

			if (!(this is ISplash)) {
				Release();
			}
		}

		public override void _EnterTree() {
			if (this is ITracked) {
				var world = Game.GetSafeArea().GetNode<Node2D>("World");
				world.AddChild(GD.Load<PackedScene>("res://src/entity/misc/Marker.tscn").Instance<Marker>().With(m => {
					m.Tracked = this;
				}));
			}
		}

		public static void Preload(Type type, int amount) {
			if (amount <= 0) return;

			if (!Pool.TryGetValue(type, out var pool)) {
				Pool[type] = pool = new Queue<Projectile>(Mathf.NearestPo2(amount));
			}

			var path = type.Namespace?.Replace("SpaceBreach.", "").Replace('.', '/');
			var res = GD.Load<PackedScene>($"res://src/{path}/{type.Name}.tscn");
			for (var i = 0; i < amount; i++) {
				pool.Enqueue(res.Instance<Projectile>());
			}
		}

		public static T Poll<T>(bool cache = true) where T : Projectile {
			if (cache) {
				Pool.TryGetValue(typeof(T), out var pool);
				if (pool == null || pool.Count == 0) {
					Preload(typeof(T), 1);
					pool = Pool[typeof(T)];
				}

				return (T) pool.Dequeue().With(p => p.Released = false);
			}

			var path = typeof(T).Namespace?.Replace("SpaceBreach.", "").Replace('.', '/');
			var res = GD.Load<PackedScene>($"res://src/{path}/{typeof(T).Name}.tscn");
			return res.Instance<T>().With(p => p.Released = false);
		}

		public void Release() {
			if (Released) return;
			Released = true;

			if (GetParent() != null) {
				GetParent().RemoveChild(this);
			}

			Pool.TryGetValue(GetType(), out var pool);
			if (pool == null || pool.Count == 0) {
				Pool[GetType()] = pool = new Queue<Projectile>(32);
			}

			pool.Enqueue(this);
		}
	}
}
