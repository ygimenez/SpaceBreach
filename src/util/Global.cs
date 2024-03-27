#define DEBUG_MODE

using Godot;
using SpaceBreach.enums;
using SpaceBreach.scene;

namespace SpaceBreach.util {
	public abstract class Global : Node {
		public static Global Instance;
		public const string CFG_PATH = "user://settings.cfg";
		public const float ACTION_SPEED = 2;

		public static readonly ConfigFile Cfg = new ConfigFile().With(cfg => {
			cfg.Load(CFG_PATH);
		});

		public override void _Ready() {
			Instance = GetNode<Global>("/root/Global");

			#if GODOT_PC
			Settings.Fields["win_res"].Action.Invoke(Cfg.GetV<Resolution>("win_res"));
			Settings.Fields["win_mode"].Action.Invoke(Cfg.GetV<WindowMode>("win_mode"));
			#else
			OS.WindowFullscreen = true;
			if (OS.HasFeature("mobile")) {
				OS.ScreenOrientation = OS.ScreenOrientationEnum.SensorLandscape
			}
			#endif

			Engine.TargetFps = 60;
			Engine.IterationsPerSecond = 100;

			#if DEBUG_MODE
			GetTree().DebugCollisionsHint = true;
			GetTree().DebugNavigationHint = true;
			#endif
		}
	}
}
