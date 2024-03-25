using SpaceBreach.entity.model;

namespace SpaceBreach.entity.enemy {
	public class Invader : Enemy {
		public Invader() : base(100) {

		}

		protected override bool Shoot() {
			return true;
		}

		protected override void Move() {

		}
	}
}
