using Godot;
using SpaceBreach.entity.interfaces;
using SpaceBreach.entity.misc;
using SpaceBreach.scene;
using SpaceBreach.util;

namespace SpaceBreach.entity.model {
	public abstract class Projectile : Area2D {
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

		public override void _PhysicsProcess(float delta) {
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

			QueueFree();
		}

		public override void _EnterTree() {
			if (this is ITracked) {
				var world = Game.GetSafeArea().GetNode<Node2D>("World");
				world.AddChild(GD.Load<PackedScene>("res://src/entity/misc/Marker.tscn").Instance<Marker>().With(m => {
					m.Tracked = this;
				}));
			}
		}
	}
}
