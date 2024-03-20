using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Godot;
using SpaceBreach.component;
using SpaceBreach.enums;
using SpaceBreach.manager;
using SpaceBreach.util;

namespace SpaceBreach.scene;

public partial class Settings : Control {
	private readonly List<ConfigField<dynamic>> _fields = new() {
		new ConfigField<dynamic>("vol_master", "Master", CfgType.PERCENT, 50),
		new ConfigField<dynamic>("vol_music", "Music", CfgType.PERCENT, 50),
		new ConfigField<dynamic>("vol_effect", "Effects", CfgType.PERCENT, 50),
		new ConfigField<dynamic>("win_mode", "Window Mode", CfgType.CYCLE, default(WindowMode),
			v => {
				switch ((WindowMode) v) {
					case WindowMode.WINDOWED:
						DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
						break;
					case WindowMode.BORDERLESS:
						DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
						break;
					case WindowMode.FULLSCREEN:
						DisplayServer.WindowSetMode(DisplayServer.WindowMode.ExclusiveFullscreen);
						break;
					default:
						throw new ArgumentOutOfRangeException(nameof(v), v, null);
				}
			}
		),
		new ConfigField<dynamic>("win_res", "Resolution", CfgType.CYCLE, default(ScreenSize),
			v => {
				switch ((ScreenSize) v) {
					case ScreenSize.R_800_600:
						DisplayServer.WindowSetSize(new Vector2I(800, 600));
						break;
					case ScreenSize.R_1024_768:
						DisplayServer.WindowSetSize(new Vector2I(1024, 768));
						break;
					case ScreenSize.R_1280_1024:
						DisplayServer.WindowSetSize(new Vector2I(1280, 1024));
						break;
					case ScreenSize.R_1366_768:
						DisplayServer.WindowSetSize(new Vector2I(1366, 768));
						break;
					case ScreenSize.R_1920_1080:
						DisplayServer.WindowSetSize(new Vector2I(1920, 1080));
						break;
					default:
						throw new ArgumentOutOfRangeException(nameof(v), v, null);
				}

				var pos = DisplayServer.ScreenGetSize() / 2 - DisplayServer.WindowGetSize() / 2;
				DisplayServer.WindowSetPosition(pos);
			}
		),
	};

	private const int LABEL_WIDTH = 150;
	private const int VALUE_WIDTH = 50;

	public override void _Ready() {
		GetNode<Button>("Back").Pressed += GetTree().Pop;
		GetNode<VBoxContainer>("ScrollContainer/CfgFields").With(box => {
			foreach (var f in _fields) {
				switch (f.Type) {
					case CfgType.PERCENT: {
						var value = Global.Cfg.GetV(f.Key, (int) f.DefaultValue);
						var lbValue = new Label {
							Text = value.ToString(),
							CustomMinimumSize = new Vector2(VALUE_WIDTH, 0)
						};

						box.AddChild(
							new HBoxContainer().With(row => {
								row.AddChild(new Label {
									Text = f.Label,
									CustomMinimumSize = new Vector2(LABEL_WIDTH, 0)
								});
								row.AddChild(
									new HSlider {
										MinValue = 0,
										MaxValue = 100,
										Value = value,
										SizeFlagsHorizontal = SizeFlags.ExpandFill
									}.With(s => {
										s.DragEnded += _ => Global.Cfg.SetV(f.Key, (int) s.Value);
										s.ValueChanged += _ => {
											lbValue.Text = ((int) s.Value).ToString();
											f.OnChange?.Invoke((int) s.Value);
										};
									})
								);
								row.AddChild(lbValue);
							})
						);
					}
						break;
					case CfgType.CYCLE: {
						var value = Global.Cfg.GetV(f.Key, 0);
						var values = f.Values();

						box.AddChild(
							new HBoxContainer().With(row => {
								row.AddChild(new Label {
									Text = f.Label,
									CustomMinimumSize = new Vector2(LABEL_WIDTH, 0)
								});
								row.AddChild(
									new Button {
										Text = ((Enum) values.GetValue(value % values.Length)).GetDescription(),
										SizeFlagsHorizontal = SizeFlags.ExpandFill,
									}.With(b => {
										b.Pressed += () => {
											value = Global.Cfg.GetV(f.Key, 0);
											var next = (Enum) values.GetValue((value + 1) % values.Length)!;

											b.Text = next.GetDescription();
											Global.Cfg.SetV(f.Key, Array.IndexOf(values, next));

											f.OnChange?.Invoke(next);
										};
									})
								);
								row.AddChild(new HSpacer());
							})
						);
					}
						break;
					case CfgType.TOGGLE:
						break;
					case CfgType.RESOLUTION:
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		});
	}
}

internal record ConfigField<T>(string Key, string Label, CfgType Type, T DefaultValue = default, Action<T> OnChange = default) {
	public Array Values() {
		return DefaultValue.GetType().IsEnum ? Enum.GetValues(DefaultValue.GetType()) : Array.Empty<dynamic>();
	}
}
