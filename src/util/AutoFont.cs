using Godot;

namespace SpaceBreach.util {
	public class AutoFont : Label {
		[Export]
		public float PercentOfParent = 1;

		[Export]
		public string TestString;

		[Export]
		public int FontSize;

		private DynamicFont _original, _font;

		public override void _Ready() {
			_original = (DynamicFont) GetFont("font");
			AddFontOverride("font", _font = (DynamicFont) _original.Duplicate());
			if (FontSize == 0) {
				FontSize = _font.Size;
			}
		}

		public override void _Process(float delta) {
			var width = _original.GetStringSize(TestString ?? Text).x;
			var box = this.FindParent<Control>().RectSize.x;
			if (PercentOfParent > 0) {
				box *= PercentOfParent;
			}

			_font.Size = (int) Mathf.Clamp(box * FontSize / width, 0, FontSize);
		}
	}
}
