using System;
using System.ComponentModel;
using Godot;
using Array = Godot.Collections.Array;
using Object = Godot.Object;

namespace SpaceBreach.util {
	public static class Utils {
		public static void Connect(this Control ctrl, string signal, Object self, string method, params object[] args) {
			ctrl.Connect(signal, self, method, new Array(args));
		}

		public static T With<T>(this T self, Action<T> action) {
			action.Invoke(self);
			return self;
		}

		public static void SetV(this ConfigFile cfg, string key, object value) {
			cfg.SetValue("CONFIG", key, value);
			cfg.Save(Global.CFG_PATH);
		}

		public static T GetV<T>(this ConfigFile cfg, string key, T defaultValue = default) {
			if (!cfg.HasSectionKey("CONFIG", key)) {
				cfg.SetV(key, defaultValue);
			}

			return (T) cfg.GetValue("CONFIG", key, defaultValue);
		}

		public static string GetDescription<T>(this T self) where T : Enum {
			var type = self.GetType();
			if (!type.IsEnum) {
				throw new ArgumentException("Self must be of Enum type", nameof(self));
			}

			var info = type.GetMember(self.ToString());
			if (info.Length > 0) {
				var attrs = info[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

				if (attrs.Length > 0) {
					return ((DescriptionAttribute) attrs[0]).Description;
				}
			}

			return self.ToString();
		}
	}
}
