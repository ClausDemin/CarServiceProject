using CarServiceProject.View.Interfaces;
using CarServiceProject.View.TableView;
using CarServiceProject.View.TableView.Enums;
using System.Drawing;

namespace CarServiceProject.View.SwitchableMenuView
{
    public class MenuItem: IUIElement
    {
        private TableCell _item;

        public MenuItem(Point position, string text, TextAlignment alignment, bool isBordered)
        {
            _item = new TableCell(position, text, alignment, isBordered);
        }

        public event Action? Clicked;

        public Point Position { get { return _item.Position; } set { _item.Position = value; } }
        public int Width => _item.Width;
        public int Height => 1;
        public int ScreenWidth => _item.ScreenWidth;

        public void Click()
        {
            Clicked?.Invoke();
        }

        public void Draw() 
        { 
            _item.Draw();
        }

        public void SetColumn(TableColumn parent) 
        {
            parent.Add(_item);
            _item.SetColumn(parent);
        }

        public void SetRow(TableRow parent)
        {
            parent.Add(_item);
            _item.SetRow(parent);
        }
    }
}
