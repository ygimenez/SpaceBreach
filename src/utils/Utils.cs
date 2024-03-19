using System;

namespace SpaceBreach.utils;

public static class Utils {
	public const string VERSION = "0.0.1-ALPHA";

	public static T With<T>(this T self, Action<T> action) {
		action.Invoke(self);
		return self;
	}
}
