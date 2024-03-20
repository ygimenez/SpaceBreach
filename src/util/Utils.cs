using System;
using Godot;

namespace SpaceBreach.util;

public static class Utils {
	public static T With<T>(this T self, Action<T> action) {
		action.Invoke(self);
		return self;
	}

	public static void SetV(this ConfigFile cfg, string key, dynamic value) {
		Console.WriteLine(value);
		cfg.SetValue("CONFIG", key, value);
		cfg.Save(Global.CFG_PATH);
	}

	public static T GetV<[MustBeVariant] T>(this ConfigFile cfg, string key, T defaultValue = default) {
		if (!cfg.HasSectionKey("CONFIG", key)) {
			cfg.SetV(key, defaultValue);
		}

		return cfg.GetValue("CONFIG", key, Variant.From(defaultValue)).As<T>();
	}

	public static void Print(string line) {
		Console.WriteLine(line);
	}
}
