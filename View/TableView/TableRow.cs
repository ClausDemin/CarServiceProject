using CarServiceProject.View.Interfaces;
using System.Drawing;

namespace CarServiceProject.View.TableView
{
    public class TableRow :IUIElement
    {
        private List<TableCell> _cells;
        private int _index;

        public TableRow(Point position, int index) 
        { 
            _cells = new List<TableCell>();
            Position = position;
            _index = index;
        }

        public event Action<int>? WidthChanged;

        public int Width => GetWidth();
        public int Height => 1;
        public int ScreenWidth => GetScreenWidth();
        public Point Position { get; set; }

        public string[] GetValues() 
        { 
            return _cells.Select(cell => cell.Text).ToArray();
        }

        public void SetValues(string[] values) 
        {
            ArgumentOutOfRangeException.ThrowIfNotEqual(values.Length, _cells.Count);

            for (int i = 0; i < _cells.Count; i++) 
            {
                _cells[i].SetValue(values[i]);
            }
        }

        public void Add(TableCell cell, int index = -1) 
        {
            if (index < 0 || index >= Height)
            {
                _cells.Add(cell);
            }
            else
            {
                _cells.Insert(index, cell);
            }

            WidthChanged?.Invoke(_index);
        }
        
        public void Draw()
        {
            Console.SetCursorPosition(Position.X, Position.Y);

            foreach (var cell in _cells)
            {
                cell.Draw();
            }
        }

        private int GetWidth() 
        {
            int width = 0;

            foreach (var cell in _cells) 
            { 
                width += cell.Width;
            }

            return width;
        }

        private int GetScreenWidth() 
        {
            int width = 0;

            foreach (var cell in _cells)
            {
                width += cell.ScreenWidth;
            }

            return width;
        }
    }
}
