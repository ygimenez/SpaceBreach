using Godot;
using SpaceBreach.enums;
using SpaceBreach.scene;

namespace SpaceBreach.util {
	public abstract class Global : Node {
		public static Global Instance;
		public const string CFG_PATH = "user://settings.cfg";

		public static readonly ConfigFile Cfg = new ConfigFile().With(cfg => {
			cfg.Load(CFG_PATH);
		});

		protected Global() {
			#if GODOT_PC
			Settings.Fields["win_res"].Action.Invoke(Cfg.GetV<Resolution>("win_res"));
			Settings.Fields["win_mode"].Action.Invoke(Cfg.GetV<WindowMode>("win_mode"));
			#else
			OS.WindowFullscreen = true;
			#endif

			Engine.TargetFps = 60;
			Engine.IterationsPerSecond = 200;
		}

		public override void _Ready() {
			Instance = GetNode<Global>("/root/Global");
		}
	}
}
