using Godot;
using SpaceBreach.enums;
using SpaceBreach.scene;

namespace SpaceBreach.util {
	public abstract class Global : Node {
		public const string CFG_PATH = "user://settings.cfg";
		public static readonly ConfigFile Cfg = new ConfigFile().With(cfg => {
			cfg.Load(CFG_PATH);
		});

		protected Global() {
			if (OS.HasFeature("pc")) {
				Settings.Fields["win_mode"].Action.Invoke(Cfg.GetV<WindowMode>("win_mode"));
				Settings.Fields["win_res"].Action.Invoke(Cfg.GetV<Resolution>("win_res"));
			} else {
				OS.WindowFullscreen = true;
			}

			Engine.TargetFps = 60;
			Engine.IterationsPerSecond = 200;
		}
	}
}
