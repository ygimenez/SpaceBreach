using Godot;
using SpaceBreach.manager;

namespace SpaceBreach.scene {
	public class Warning : Label {
		public override void _Ready() {

		}

		public void _PlayWarning() {
			Audio.StopMusic();
			Audio.Cue("res://assets/sounds/warning.wav");
		}

		public void _BossMusic() {
			Audio.PlayMusic("res://assets/sounds/music/boss.tres");
		}
	}
}
