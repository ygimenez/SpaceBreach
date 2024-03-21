using Godot;

namespace SpaceBreach.util {
	public static class Global {
		public const string CFG_PATH = "user://settings.cfg";

		public static readonly ConfigFile Cfg = new ConfigFile().With(cfg => {
			cfg.Load(CFG_PATH);
		});
	}
}
