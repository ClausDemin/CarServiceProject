using CarServiceProject.View.Interfaces;
using System.Drawing;

namespace CarServiceProject.View.TableView
{
    public class TableColumn: IUIElement
    {
        private List<TableCell> _cells;
        private int _index;
        private Point _position;
        private bool _isDisplaying;

        public TableColumn(Point position, int index) 
        { 
            _cells = new List<TableCell>();
            _index = index;
            _position = position;
        }

        public event Action? HeightChanged;
        public event Action<int>? WidthChanged;

        public int Width => GetWidth();
        public int ScreenWidth => GetScreenWidth();
        public int Height => _cells.Count;
        public Point Position 
        { 
            get 
            { 
                return _position; 
            } 
            set 
            { 
                _position = value; 
                UpdateCellsPosition(); 
            } 
        }

        public void Draw() 
        {
            Clear();

            for (int i = 0; i < Height; i++) 
            {
                _cells[i].Draw();
            }

            _isDisplaying = true;
        }

        public void Clear() 
        {
            foreach (var cell in _cells) 
            { 
                cell.Clear();
            }

            _isDisplaying = false;
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

            if (cell.Width >= Width) 
            {
                WidthChanged?.Invoke(index);
            }

            cell.ValueChanged += () => OnValueChanged(cell);

            UpdateCellsPosition();

            HeightChanged?.Invoke();
        }

        public void SetValue(int index, string value) 
        {
            ArgumentOutOfRangeException.ThrowIfNegative(index);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, _cells.Count);

            if (value.Length > Width)
            {
                _cells[index].SetValue(value);
                
                Draw();

                WidthChanged?.Invoke(_index + 1);
            }
            else 
            {
                _cells[index].SetValue(value);
            }
        }

        public void SetValues(string[] values) 
        {
            ArgumentOutOfRangeException.ThrowIfNotEqual(values.Length, _cells.Count);

            for (int i = 0; i < values.Length; i++)
            {
                _cells[i].SetValue(values[i]);
            }
        }

        public IEnumerator<TableCell> GetEnumerator() 
        {
            foreach (var cell in _cells) 
            { 
                yield return cell;
            }
        }

        private void UpdateCellsPosition()
        {
            for (int i = 0; i < _cells.Count; i++)
            {
                _cells[i].Position = new Point(Position.X, Position.Y + i);
            }
        }

        private int GetWidth() 
        {
            int width = _cells.Max(cell => cell.Text.Length);

            return width;
        }

        private int GetScreenWidth() 
        {
            int width = _cells.Max(cell => cell.ScreenWidth);

            return width;
        }

        private void OnValueChanged(TableCell cell) 
        {
            if (_isDisplaying) 
            {
                cell.Clear();
                cell.Draw();
            }
        }
    }
}
