using System.Threading.Tasks;
using Godot;
using Godot.Collections;
using SpaceBreach.entity.interfaces;
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

		public Vector2 Size => GetNode<Sprite>("Sprite").GetRect().Size;

		protected Array Cannons {
			get {
				var cannons = GetNode("Cannons");
				if (cannons != null && cannons.GetChildCount() > 0) {
					return cannons.GetChildren();
				}

				return new Array();
			}
		}

		private SceneTreeTween _tween;
		protected uint Hp;
		public float RawSpeed;
		public float RawActionSpeed = 1;
		public new bool Visible;

		protected Entity(uint baseHp, float speed = 1) {
			BaseHp = Hp = baseHp;
			Speed = speed;
		}

		public override void _Ready() {
			if (this is IBoss) {
				var bar = GetGame().GetNode<ProgressBar>("MaxSizeContainer3/BossHp");

				_tween?.Kill();
				_tween = CreateTween();
				_tween.TweenProperty(bar, "value", Hp, 1);
			}
		}

		protected Game GetGame() {
			return GetNode<Game>("/root/Control");
		}

		public uint GetHp() {
			return Hp;
		}

		public void AddHp(Entity source, long value) {
			if (this is Enemy && !Visible) return;
			if (this is IBoss b && !b.Ready) return;

			if (source != this && value < 0) {
				OnDamaged(source, value);
			}

			Hp = (uint) Mathf.Clamp(Hp + value, 0, BaseHp);
			if (this is IBoss) {
				var bar = GetGame().GetNode<ProgressBar>("MaxSizeContainer3/BossHp");

				_tween?.Kill();
				_tween = CreateTween();
				_tween.TweenProperty(bar, "value", Hp, 1);
			}

			if (Hp == 0) {
				OnDestroy().ContinueWith(_ => QueueFree());
			}
		}

		protected virtual void OnDamaged(Entity by, long value) {
		}

		protected virtual Task OnDestroy() {
			return Task.CompletedTask;
		}
	}
}
