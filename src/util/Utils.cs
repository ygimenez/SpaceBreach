using System;

namespace SpaceBreach.util;

public static class Utils {
	public static T With<T>(this T self, Action<T> action) {
		action.Invoke(self);
		return self;
	}
}
