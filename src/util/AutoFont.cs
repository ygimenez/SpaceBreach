using Godot;

namespace SpaceBreach.util {
	public class AutoFont : Label {
		[Export]
		public float PercentOfParent;

		[Export]
		public string TestString;

		private DynamicFont _original, _font;
		private int _fontSize;

		public override void _Ready() {
			_original = (DynamicFont) GetFont("font");
			AddFontOverride("font", _font = (DynamicFont) _original.Duplicate());
			_fontSize = _font.Size;
		}

		public override void _Process(float delta) {
			var width = _original.GetStringSize(TestString ?? Text).x;
			var box = this.FindParent<Control>().RectSize.x;
			if (PercentOfParent > 0) {
				box *= PercentOfParent;
			}

			if (width > box) {
				_font.Size = (int) (box * _fontSize / width).Clamp(0, _fontSize);
			}
		}
	}
}
