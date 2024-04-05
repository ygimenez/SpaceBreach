using System.Threading.Tasks;
using Godot;
using Godot.Collections;
using SpaceBreach.entity.interfaces;
using SpaceBreach.entity.misc;
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
			get => RawActionSpeed * Global.ACTION_SPEED * (this is Enemy ? Engine.TimeScale : 1);
			set => RawActionSpeed = value;
		}

		[Signal]
		public delegate void Death(Entity entity);

		public Vector2 Size => GetNode<Sprite>("Sprite").GetRect().Size;

		protected Game Game => GetNode<Game>("/root/Control");

		public Array Cannons {
			get {
				var cannons = GetNode("Cannons");
				if (cannons != null && cannons.GetChildCount() > 0) {
					return cannons.GetChildren();
				}

				return new Array();
			}
		}

		protected uint Hp;
		public float RawSpeed;
		public float RawActionSpeed = 1;
		public bool Appeared { get; set; }
		public bool Dying;

		protected Entity(uint baseHp, float speed = 1) {
			BaseHp = Hp = baseHp;
			Speed = speed;
		}

		public override void _Ready() {
			if (this is IBoss) {
				var bar = Game.GetNode<ProgressBar>("MaxSizeContainer3/BossHp");

				var tween = CreateTween();
				tween.TweenProperty(bar, "value", Hp, 1);
			}
		}

		public uint GetHp() {
			return Hp;
		}

		public virtual void AddHp(Entity source, long value) {
			if (Dying) return;
			if (this is Enemy && !Visible) return;
			if (this is IBoss b && !b.Ready) return;

			if (source != this && value < 0) {
				OnDamaged(source, value);
			}

			Hp = (uint) Mathf.Clamp(Hp + value, 0, BaseHp);
			if (this is IBoss) {
				var bar = Game.GetNode<ProgressBar>("MaxSizeContainer3/BossHp");

				var tween = CreateTween();
				tween.TweenProperty(bar, "value", Hp, 1);
			}

			if (Hp == 0) {
				Dying = true;
				Kill();
			}
		}

		public async void Kill() {
			await OnDestroy();
			if (this is IBoss) {
				Game.Level++;
			}

			QueueFree();
		}

		protected virtual void OnDamaged(Entity by, long value) {
		}

		protected virtual Task OnDestroy() {
			return Task.CompletedTask;
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
