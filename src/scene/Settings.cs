using System;
using System.Collections.Generic;
using Godot;
using SpaceBreach.component;
using SpaceBreach.enums;
using SpaceBreach.manager;
using SpaceBreach.util;

namespace SpaceBreach.scene {
	public class Settings : Control {
		private static readonly Dictionary<string, ConfigField> Fields = new Dictionary<string, ConfigField> {
			{ "vol_master", new ConfigField("vol_master", "Master", CfgType.PERCENT, 50) },
			{ "vol_music", new ConfigField("vol_music", "Music", CfgType.PERCENT, 50) },
			{ "vol_effect", new ConfigField("vol_effect", "Effects", CfgType.PERCENT, 50) }, {
				"win_mode", new ConfigField("win_mode", "Window Mode", CfgType.CYCLE, default(WindowMode),
					v => {
						switch ((WindowMode) v) {
							case WindowMode.WINDOWED:
								OS.WindowBorderless = false;
								OS.WindowFullscreen = false;

								Fields["win_res"].OnChange.Invoke(Global.Cfg.GetV("win_res", 0));
								break;
							case WindowMode.BORDERLESS:
								OS.WindowBorderless = true;
								OS.WindowFullscreen = false;

								OS.WindowPosition = new Vector2();
								OS.WindowSize = OS.GetScreenSize();
								break;
							case WindowMode.FULLSCREEN:
								OS.WindowBorderless = false;
								OS.WindowFullscreen = true;
								break;
							default:
								throw new ArgumentOutOfRangeException(nameof(v), v, null);
						}
					}
				)
			}, {
				"win_res", new ConfigField("win_res", "Resolution", CfgType.CYCLE, default(ScreenSize),
					v => {
						switch ((ScreenSize) v) {
							case ScreenSize.R_800_600:
								OS.WindowSize = new Vector2(800, 600);
								break;
							case ScreenSize.R_1024_768:
								OS.WindowSize = new Vector2(1024, 768);
								break;
							case ScreenSize.R_1280_1024:
								OS.WindowSize = new Vector2(1280, 1024);
								break;
							case ScreenSize.R_1366_768:
								OS.WindowSize = new Vector2(1366, 768);
								break;
							case ScreenSize.R_1920_1080:
								OS.WindowSize = new Vector2(1920, 1080);
								break;
							default:
								throw new ArgumentOutOfRangeException(nameof(v), v, null);
						}

						var pos = OS.GetScreenSize() / 2 - OS.WindowSize / 2;
						OS.WindowPosition = pos;
					}
				)
			},
		};

		private const int LABEL_WIDTH = 150;
		private const int VALUE_WIDTH = 50;

		public override void _Ready() {
			GetNode<Button>("Back").Connect("pressed", this, nameof(_BackPressed));
			GetNode<VBoxContainer>("ScrollContainer/CfgFields").With(box => {
				foreach (var v in Fields.Values) {
					switch (v.Type) {
						case CfgType.PERCENT: {
							var value = Global.Cfg.GetV(v.Key, (int) v.DefaultValue);

							box.AddChild(
								new HBoxContainer().With(row => {
									row.AddChild(new Label {
										Text = v.Label,
										RectMinSize = new Vector2(LABEL_WIDTH, 0)
									});
									row.AddChild(
										new HSlider {
											MinValue = 0,
											MaxValue = 100,
											Value = value,
											SizeFlagsHorizontal = (int) SizeFlags.ExpandFill
										}.With(s => {
											s.Connect("drag_ended", this, nameof(_SliderDragEnded), v.Key, s);
											s.Connect("value_changed", this, nameof(_SliderValueChanged), v.Key, s);
										})
									);
									row.AddChild(new Label {
										Name = $"SliderCaption_{v.Key}",
										Text = value.ToString(),
										RectMinSize = new Vector2(VALUE_WIDTH, 0)
									});
								})
							);
						}
							break;
						case CfgType.CYCLE: {
							var value = Global.Cfg.GetV(v.Key, 0);
							var values = v.Values();

							box.AddChild(
								new HBoxContainer().With(row => {
									row.AddChild(new Label {
										Text = v.Label,
										RectMinSize = new Vector2(LABEL_WIDTH, 0)
									});
									row.AddChild(
										new Button {
											Text = ((Enum) values.GetValue(value % values.Length)).GetDescription(),
											SizeFlagsHorizontal = (int) SizeFlags.ExpandFill,
										}.With(b => {
											b.Connect("pressed", this, nameof(_TogglePressed), v.Key, b);
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

		private void _SliderDragEnded(bool _, string key, Range slider) {
			var field = Fields[key];
			Global.Cfg.SetV(field.Key, (int) slider.Value);
		}

		private void _SliderValueChanged(float _, string key, Range slider) {
			var field = Fields[key];
			slider.GetParent().GetNode<Label>($"SliderCaption_{key}").Text = ((int) slider.Value).ToString();
			field.OnChange?.Invoke((int) slider.Value);
		}

		private void _TogglePressed(string key, Button button) {
			var field = Fields[key];
			var value = Global.Cfg.GetV(field.Key, 0);
			var values = field.Values();
			var next = (Enum) values.GetValue((value + 1) % values.Length);

			button.Text = next.GetDescription();
			Global.Cfg.SetV(field.Key, Array.IndexOf(values, next));

			field.OnChange?.Invoke(next);
		}

		private void _BackPressed() {
			GetTree().Pop();
		}
	}

	internal class ConfigField {
		public readonly string Key;
		public readonly string Label;
		public readonly CfgType Type;
		public readonly object DefaultValue;
		public readonly Action<object> OnChange;

		public ConfigField(string key, string label, CfgType type, object defaultValue = default, Action<object> onChange = default) {
			Key = key;
			Label = label;
			Type = type;
			DefaultValue = defaultValue;
			OnChange = onChange;
		}

		public Array Values() {
			return DefaultValue.GetType().IsEnum ? Enum.GetValues(DefaultValue.GetType()) : default;
		}
	}
}
