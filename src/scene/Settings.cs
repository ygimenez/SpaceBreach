using System;
using System.Collections.Generic;
using Godot;
using SpaceBreach.manager;
using SpaceBreach.util;

namespace SpaceBreach.scene;

public partial class Settings : Control {
	private readonly List<(string, string)> _fields = new() {
		("vol_master", "vol_master")
	};

	public override void _Ready() {
		GetNode<Button>("Back").Pressed += GetTree().Pop;
		GetNode<VBoxContainer>("ScrollContainer/CfgFields").With(box => {
			box.AddChild(new HSlider {
				MinValue = 0,
				MaxValue = 100,
				Value = Global.Cfg.GetV("vol_master", 50)
			}.With(s => {
				s.DragEnded += _ => Global.Cfg.SetV("vol_master", s.Value);
			}));
		});
	}
}
