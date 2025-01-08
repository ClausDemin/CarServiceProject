using CarServiceProject.View.SwitchableMenuView;
using CarServiceProject.View.TableView.Enums;
using CarServiceProject.View.Utils;
using System.Drawing;

namespace CarServiceProject.View.Infrastructure
{
    public class SwitchableMenuBuilder
    {
        private List<MenuItem> _items = new List<MenuItem>();

        private Point _position;

        public SwitchableMenuBuilder(Point position) 
        { 
            _position = position;
        }

        public SwitchableMenuBuilder Reset()
        { 
            _items.Clear();

            return this;
        }

        public SwitchableMenuBuilder AddItem(string text, TextAlignment alignment, bool isBordered ,Action onClick) 
        {
            var position = _position;

            if (_items.Count > 0) 
            {
                position = UIUtils.GetPositionBelow(_items.Last());
            }

            var item = new MenuItem(position, text, alignment, isBordered);
            item.Clicked += onClick;

            _items.Add(item);

            return this;
        }

        public SwitchableMenu Build() 
        {
            var menu = new SwitchableMenu(_position);

            foreach (var item in _items) 
            {
                menu.Add(item);
            }

            return menu;
        }
    }
}
