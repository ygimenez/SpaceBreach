using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using SpaceBreach.enums;
using SpaceBreach.util;

namespace SpaceBreach.scene {
	public abstract class Settings : BaseMenu {
		public static readonly Dictionary<string, ConfigField> Fields = new Dictionary<string, ConfigField> {
			{ "vol_master", new ConfigField("vol_master", "Master", CfgType.PERCENT, 50) },
			{ "vol_music", new ConfigField("vol_music", "Music", CfgType.PERCENT, 50) },
			{ "vol_effect", new ConfigField("vol_effect", "Effects", CfgType.PERCENT, 50) }, {
				"win_mode", new ConfigField("win_mode", "Window Mode", CfgType.CYCLE, default(WindowMode),
					v => {
						switch (v) {
							case WindowMode.WINDOWED:
								OS.WindowBorderless = false;
								OS.WindowFullscreen = false;
								break;
							case WindowMode.BORDERLESS:
								OS.WindowBorderless = true;
								OS.WindowFullscreen = false;

								OS.WindowPosition = new Vector2();
								OS.WindowSize = OS.GetScreenSize() - new Vector2(0, 1);
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
				"win_res", new ConfigField("win_res", "Resolution", CfgType.CYCLE, default(Resolution),
					v => {
						var mode = Global.Cfg.GetV("win_mode", 0).ToEnum<WindowMode>();
						Global.Instance.GetViewport().SetSizeOverride(false);

						var dims = ((Resolution) v).GetDescription().Split("x");
						var size = new Vector2(int.Parse(dims[0]), int.Parse(dims[1]));
						if (mode == WindowMode.WINDOWED) {
							OS.WindowSize = size;

							var pos = OS.GetScreenSize() / 2 - OS.WindowSize / 2;
							OS.WindowPosition = pos;
						}
					}
				)
			}
		};

		public override void _Ready() {
			base._Ready();

			GetNode<GridContainer>("MaxSizeContainer/ScrollContainer/CfgFields").With(box => {
				foreach (var v in Fields.Values.Where(v => OS.HasFeature("pc") || !v.Key.StartsWith("win_"))) {
					switch (v.Type) {
						case CfgType.PERCENT: {
							var value = Global.Cfg.GetV(v.Key, (int) v.DefaultValue);

							box.AddChild(new Label {
								Text = v.Label,
							});
							box.AddChild(
								new HSlider {
									Name = v.Key,
									MinValue = 0,
									MaxValue = 100,
									Value = value,
									SizeFlagsHorizontal = (int) SizeFlags.ExpandFill
								}.With(s => {
									s.Connect("drag_ended", this, nameof(_SliderDragEnded), v.Key, s);
									s.Connect("value_changed", this, nameof(_SliderValueChanged), v.Key, s);
								})
							);
							box.AddChild(new Label {
								Name = $"SliderCaption_{v.Key}",
								Text = value.ToString().PadRight(3)
							});
						}
							break;
						case CfgType.CYCLE: {
							var value = Global.Cfg.GetV(v.Key, 0);
							var values = v.Values();

							box.AddChild(new Label {
								Text = v.Label
							});
							box.AddChild(
								new Button {
									Name = v.Key,
									Text = (value % values.Length).ToEnum(v.DefaultValue.GetType()).GetDescription(),
									SizeFlagsHorizontal = (int) SizeFlags.ExpandFill
								}.With(b => {
									b.Connect("gui_input", this, nameof(_TogglePressed), v.Key, b);
								})
							);
							box.AddChild(new Control());
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

		public override void _Process(float delta) {
			GetNode<GridContainer>("MaxSizeContainer/ScrollContainer/CfgFields").With(box => {
				box.GetNode<Button>("win_res").Disabled = Global.Cfg.GetV("win_mode", 0) != WindowMode.WINDOWED.Ordinal();
			});
		}

		private void _SliderDragEnded(bool _, string key, Slider slider) {
			if (!slider.Editable) return;

			var field = Fields[key];
			Global.Cfg.SetV(field.Key, (int) slider.Value);
		}

		private void _SliderValueChanged(float _, string key, Slider slider) {
			if (!slider.Editable) return;

			var field = Fields[key];
			slider.GetNode<Label>($"../SliderCaption_{key}").Text = ((int) slider.Value).ToString();
			field.Action?.Invoke((int) slider.Value);
		}

		private void _TogglePressed(InputEvent evt, string key, Button button) {
			if (!evt.IsPressed() || button.Disabled) return;

			var field = Fields[key];
			var value = Global.Cfg.GetV(field.Key, 0);
			var values = field.Values();

			Enum next = null;
			if (evt is InputEventMouseButton mb) {
				if (mb.ButtonIndex == ButtonList.Right.Ordinal()) {
					var idx = value - 1;
					if (idx < 0) {
						idx = idx % values.Length + values.Length;
					}

					next = (idx % values.Length).ToEnum(field.DefaultValue.GetType());
				}
			}

			if (next == null) {
				next = ((value + 1) % values.Length).ToEnum(field.DefaultValue.GetType());
			}

			button.Text = next.GetDescription();
			Global.Cfg.SetV(field.Key, next.Ordinal());

			field.Action?.Invoke(next);
		}

		public static void Apply() {
			Fields["win_mode"].Action.Invoke(Global.Cfg.GetV<WindowMode>("win_mode"));
			Fields["win_res"].Action.Invoke(Global.Cfg.GetV<Resolution>("win_res"));
		}
	}

	public class ConfigField {
		public readonly string Key;
		public readonly string Label;
		public readonly CfgType Type;
		public readonly object DefaultValue;
		public readonly Action<object> Action;

		public ConfigField(string key, string label, CfgType type, object defaultValue = default, Action<object> action = default) {
			Key = key;
			Label = label;
			Type = type;
			DefaultValue = defaultValue;
			Action = action;
		}

		public Array Values() {
			return DefaultValue.GetType().IsEnum ? Enum.GetValues(DefaultValue.GetType()) : default;
		}
	}
}
