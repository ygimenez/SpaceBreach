using System.Linq;
using Godot;
using Godot.Collections;
using SpaceBreach.entity.model;
using SpaceBreach.manager;
using SpaceBreach.util;

namespace SpaceBreach.scene {
	public abstract class Hangar : BaseMenu {
		private readonly Dictionary<Player, Button> _buttons = new Dictionary<Player, Button>();
		private Player _base, _selected;

		public override void _Ready() {
			base._Ready();

			_base = GD.Load<PackedScene>("res://src/entity/player/Fighter.tscn").Instance<Player>();
			var ships = typeof(Player).Assembly
				.GetTypes()
				.Where(t => t.IsSubclassOf(typeof(Player)))
				.Select(t => Utils.Load(t).Instance<Player>())
				.ToList();

			GetNode<Control>("Ships").With(c => {
				var normal = (StyleBox) GD.Load<StyleBox>("res://assets/textures/panel_normal.tres").Duplicate();
				normal.ContentMarginTop = normal.ContentMarginRight = normal.ContentMarginBottom = normal.ContentMarginLeft = 20;

				var hover = (StyleBox) GD.Load<StyleBox>("res://assets/textures/panel_highlight.tres").Duplicate();
				hover.ContentMarginTop = hover.ContentMarginRight = hover.ContentMarginBottom = hover.ContentMarginLeft = 20;

				var pressed = (StyleBox) GD.Load<StyleBox>("res://assets/textures/panel_highlight.tres").Duplicate();
				pressed.ContentMarginTop = pressed.ContentMarginRight = pressed.ContentMarginBottom = pressed.ContentMarginLeft = 10;

				var disabled = (StyleBox) GD.Load<StyleBox>("res://assets/textures/panel_disabled.tres").Duplicate();
				disabled.ContentMarginTop = disabled.ContentMarginRight = disabled.ContentMarginBottom = disabled.ContentMarginLeft = 20;

				foreach (var ship in ships) {
					c.AddChild(new Button {
						Icon = ship.GetNode<Sprite>("Sprite").Texture,
						RectMinSize = new Vector2(80, 80),
						ExpandIcon = true,
						ToggleMode = true
					}.With(b => {
						b.AddStyleboxOverride("normal", normal);
						b.AddStyleboxOverride("hover", hover);
						b.AddStyleboxOverride("pressed", pressed);
						b.AddStyleboxOverride("disabled", disabled);

						b.Connect("pressed", this, nameof(_SelectShip), ship);
						_buttons[ship] = b;
					}));
				}
			});

			GetNode<Button>("Start").Connect("pressed", this, nameof(_StartPressed));
		}

		public override void _Process(float delta) {
			foreach (var e in _buttons) {
				e.Value.Pressed = _selected == e.Key;
			}

			GetNode<Button>("Start").Disabled = _selected == null;
			GetNode<Control>("Ships").With(c => {
				c.RectSize = new Vector2(RectSize.x / 3, c.RectSize.y);
			});
		}

		public void _StartPressed() {
			Game.Player = _selected;
			GetTree().Append("res://src/scene/Game.tscn");
		}

		public void _SelectShip(Player ship) {
			_selected = ship == _selected ? null : ship;

			if (_selected != null) {
				GetNode<RichTextLabel>("MaxSizeContainer/Description").With(l =>
					l.BbcodeText = $@"
					[b]{ship.GetType().Name}[/b]

					{ship.Description}

					Armor:        [{Utils.PrcntBar(ship.BaseHp / (float) _base.BaseHp / 2, 10)}]
					Damage:       [{Utils.PrcntBar(ship.DamageMult / _base.DamageMult / 2, 10)}]
					Projectiles:  [{Utils.PrcntBar(ship.Projectiles * ship.Cannons.Count / (_base.Projectiles * _base.Cannons.Count) / 2, 10)}]
					Fire rate:    [{Utils.PrcntBar(ship.AttackRate / _base.AttackRate / 2, 10)}]
					SP recovery:  {(ship.SpecialRate > 0 ? $"[{Utils.PrcntBar(ship.SpecialRate / _base.SpecialRate / 2, 10)}]" : "N/A")}
					Speed:        [{Utils.PrcntBar(ship.Speed / _base.Speed / 2, 10)}]
					".Trim().Unindent()
				);
			} else {
				GetNode<RichTextLabel>("Description").Text = "";
			}
		}

		public override void _ExitTree() {
			_base.QueueFree();
		}
	}
}
