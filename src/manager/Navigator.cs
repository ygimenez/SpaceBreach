using System;
using System.Collections.Generic;
using Godot;
using SpaceBreach.util;

namespace SpaceBreach.manager;

public static class Navigator {
	private static readonly Stack<string> Stack = new Stack<string>().With(s => s.Push("res://src/scene/Main.tscn"));

	public static void Append(this SceneTree tree, string screen) {
		tree.ChangeSceneToPacked(ResourceLoader.Load<PackedScene>(screen));
		Stack.Push(screen);
	}

	public static void Pop(this SceneTree tree) {
		if (Stack.Count <= 1) return;
		else if (Stack.TryPop(out var current) && Stack.TryPeek(out current)) {
			tree.Append(current);
		}
	}

	public static void PopUntil(this SceneTree tree, Predicate<string> condition) {
		while (Stack.TryPeek(out var next)) {
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
