using CarServiceProject.View.Interfaces;
using CarServiceProject.View.SwitchableMenuView.Enums;
using CarServiceProject.View.TableView;
using CarServiceProject.View.Utils;
using System.Drawing;

namespace CarServiceProject.View.SwitchableMenuView
{
    public class SwitchableMenu : IUIElement
    {
        private TableColumn _names;
        private TableColumn _cursor;
        private List<MenuItem> _items;

        private MenuItem? _current;
        private int _currentIndex;

        public SwitchableMenu(Point position, char cursor = '>') 
        {
            _cursor = new TableColumn(position, 0);
            _names = new TableColumn(position, 1);
            _items = new List<MenuItem>();

            Position = position;
            Cursor = cursor;
        }

        public char Cursor { get; set; }
        public Point Position { get ; set; }
        public int Width => GetWidth();
        public int Height => GetHeight();
        public int ScreenWidth => GetScreenWidth();
        public int Current => _currentIndex;

        public void Add(MenuItem item) 
        {
            _items.Add(item);
            item.SetColumn(_names);

            Add();

            if (_current == null)
            {
                _current = item;
                _currentIndex = 0;
            }
        }

        public void Click() 
        {
            if (_current != null) 
            { 
                _current.Click();
            }
        }

        public void Draw() 
        {
            if (_items.Count > 0) 
            { 
                _cursor.Draw();
                _names.Draw();
            }
        }

        public void Clear() 
        { 
            _names.Clear();
            _cursor.Clear();
        }

        public void MoveCursor(CursorMovement direction) 
        {
            if (IsCursorInBounds(_currentIndex, direction)) 
            {
                _cursor.SetValue(_currentIndex, string.Empty);
                _currentIndex += (int) direction;
                _cursor.SetValue(_currentIndex, Cursor.ToString());
                _current = _items[_currentIndex];
            }
        }

        private void Add()
        {
            var cell = new TableCell(_cursor.Position, null, TableView.Enums.TextAlignment.Right, true);
            _cursor.Add(cell);
            cell.SetColumn(_cursor);

            if (_current == null)
            {
                cell.SetValue(Cursor.ToString());
                _names.Position = UIUtils.GetPositionAfter(_cursor);
            }
        }

        private bool IsCursorInBounds(int index, CursorMovement direction) 
        { 
            var nextIndex = index + (int) direction;

            return nextIndex >= 0 && nextIndex < _items.Count;
        }

        private int GetWidth() 
        { 
            return _cursor.Width + _names.Width;
        }

        private int GetScreenWidth() 
        { 
            return _cursor.ScreenWidth + _names.ScreenWidth;
        }

        private int GetHeight() 
        {
            return _cursor.Height;
        }
    }
}
