using System;
using System.ComponentModel;
using System.Linq;
using Godot;
using SpaceBreach.entity.model;
using Array = Godot.Collections.Array;
using Object = Godot.Object;

namespace SpaceBreach.util {
	public static class Utils {
		public static readonly RandomNumberGenerator Rng = new RandomNumberGenerator();

		public static void Connect(this Node node, string signal, Object self, string method, params object[] args) {
			node.Connect(signal, self, method, new Array(args));
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

		public static T Cast<T>(this Type type, object obj) {
			return (T) Convert.ChangeType(obj, type);
		}

		public static T ToEnum<T>(this int ordinal) where T : Enum {
			return (T) Enum.GetValues(typeof(T)).GetValue(ordinal);
		}

		public static Enum ToEnum(this int ordinal, Type type) {
			return type.Cast<Enum>(Enum.GetValues(type).GetValue(ordinal));
		}

		public static int Ordinal(this Enum val) {
			return (int) (IConvertible) val;
		}

		public static int IntValue(this bool value) {
			return value ? 1 : 0;
		}

		public static int Clamp(this int val, int min, int max) {
			if (val < min) return min;
			return val > max ? max : val;
		}

		public static float Clamp(this float val, float min, float max) {
			if (val < min) return min;
			return val > max ? max : val;
		}

		public static float ToRadians(this float deg) {
			return deg * (Mathf.Pi / 180);
		}

		public static float ToDegrees(this float rad) {
			return rad * 180 / Mathf.Pi;
		}

		public static bool IsBetween(this int val, int min, int max) {
			return val >= min && val <= max;
		}

		public static bool IsBetween(this float val, float min, float max) {
			return val >= min && val <= max;
		}

		public static bool EqualsAny<T>(this T self, params T[] args) {
			return args.Any(arg => Equals(self, arg));
		}

		public static T FindParent<T>(this Node node, Predicate<T> condition = null) where T : Node {
			var curr = node;
			while (true) {
				curr = curr.GetParent();
				switch (curr) {
					case T t when (condition?.Invoke(t) ?? true):
						return t;
					case null:
						return null;
				}
			}
		}

		public static void FindParent<T>(this Node node, ref T output, Predicate<Node> condition = null) where T : Node {
			output = node.FindParent<T>(condition);
		}

		public static Node FindParent(this Node node, Predicate<Node> condition = null) {
			return node.FindParent<Node>(condition);
		}

		public static void FindParent(this Node node, ref Node output, Predicate<Node> condition = null) {
			node.FindParent<Node>(ref output, condition);
		}

		public static string PrcntBar(float prcnt, int width) {
			var left = (int) (width * prcnt);
			return new string('|', left) + new string(' ', width - left);
		}
	}
}
