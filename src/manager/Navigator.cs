using System;
using System.Collections.Generic;
using Godot;
using SpaceBreach.util;

namespace SpaceBreach.manager {
	public static class Navigator {
		private static readonly Stack<string> Stack = new Stack<string>().With(s => s.Push("res://src/scene/Main.tscn"));

		public static void Append(this SceneTree tree, string screen) {
			tree.ChangeSceneTo(GD.Load<PackedScene>(screen));
			Stack.Push(screen);
		}

		public static void Pop(this SceneTree tree) {
			if (Stack.Count <= 1) return;

			if (Stack.Count > 1) {
				Stack.Pop();
				tree.ChangeSceneTo(GD.Load<PackedScene>(Stack.Peek()));
			}
		}

		public static void PopUntil(this SceneTree tree, Predicate<string> condition) {
			while (Stack.Count > 1) {
				var next = Stack.Peek();
				if (!condition.Invoke(next)) {
					Stack.Pop();
					tree.Pop();
				}
			}
		}

		public static void Quit(this SceneTree tree) {
			tree.Quit();
		}
	}
}
