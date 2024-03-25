using Godot;
using SpaceBreach.scene;
using SpaceBreach.util;

namespace SpaceBreach.entity.model {
	public abstract class Enemy : Entity {
		[Export]
		public float AttackRate;

		public Cooldown Cooldown;

		private readonly bool _drop;

		protected Enemy(uint hp, float attackRate = 1) : base(hp) {
			AttackRate = attackRate;

			_drop = Utils.Rng.Randfn() > 1 - Mathf.Min(GetCost() * 0.001f, 0.2f);
			if (_drop || true) {
				Modulate = Colors.Yellow;
			}
		}

		public override void _Ready() {
			base._Ready();
			var game = GetNode<Game>("/root/Control");

			BaseHp = Hp = (BaseHp + game.GetScore() / 2) * game.GetLevel();
			Cooldown = new Cooldown(game, (uint) (500 / (AttackRate + 0.2f * game.GetLevel())));
		}

		public override void _Process(float delta) {
			if (Cooldown.Ready() && Shoot()) {
				Cooldown.Use();
			}
		}

		public override void _PhysicsProcess(float delta) {
			Move();
		}

		public uint GetCost() {
			return (uint) (Hp * AttackRate);
		}

		protected abstract bool Shoot();

		protected abstract void Move();
	}
}
