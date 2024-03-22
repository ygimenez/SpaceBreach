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

			if (Stack.Count > 0) {
				Stack.Pop();
				if (Stack.Count > 0) {
					tree.Append(Stack.Peek());
				}
			}
		}

		public static void PopUntil(this SceneTree tree, Predicate<string> condition) {
			while (Stack.Count > 0) {
				var next = Stack.Peek();
				if (Stack.Count > 1 && !condition.Invoke(next)) {
					tree.Pop();
				} else {
					break;
				}
			}
		}

		public static void Quit(this SceneTree tree) {
			tree.Quit();
		}
	}
}
